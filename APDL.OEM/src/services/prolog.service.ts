import { exec } from 'child_process';
import { promisify } from 'util';
import * as fs from 'fs/promises';
import * as path from 'path';
import { VesselVisitNotificationDto } from './vesselVisitNotification.service';

const execAsync = promisify(exec);

export interface PrologVesselFact {
  vesselLabel: string;
  vvnId: string;
  arrivalTime: number;
  departureTime: number;
  unloadTime: number;
  loadTime: number;
}

export interface PrologScheduleResult {
  vesselLabel: string;
  unloadStartTime: number;
  loadEndTime: number;
  dockId?: string;
  craneId?: string;
  staffId?: string;
  storageId?: string;
  timeFactor?: number;
  delay: number;
}

export class PrologService {
  private prologScriptPath: string;
  private tempDir: string;

  constructor() {
    this.prologScriptPath = path.join(
      __dirname,
      '../../../../APDL.Planning/algorithms/original.pl'
    );
    this.tempDir = path.join(__dirname, '../../temp');
  }

  convertVVNsToPrologFacts(
    vvns: VesselVisitNotificationDto[],
    baseTime: Date
  ): PrologVesselFact[] {
    const facts: PrologVesselFact[] = [];

    vvns.forEach((vvn, index) => {
      const vesselLabel = `v${String.fromCharCode(97 + index)}`;

      const arrivalDate = new Date(vvn.expectedArrival);
      const departureDate = new Date(vvn.expectedDeparture);
      
      const arrivalTime = this.dateToTimeUnits(arrivalDate, baseTime);
      const departureTime = this.dateToTimeUnits(departureDate, baseTime);

      if (arrivalTime < 0 || departureTime < 0) {
        throw new Error(
          `Invalid time units for VVN ${vvn.id}: arrivalTime=${arrivalTime}, departureTime=${departureTime}`
        );
      }

      if (departureTime <= arrivalTime) {
        throw new Error(
          `Invalid time units for VVN ${vvn.id}: departureTime (${departureTime}) must be greater than arrivalTime (${arrivalTime})`
        );
      }

      const CARGO_VOLUME_TO_TIME_FACTOR = 400;
      const unloadTime = Math.max(1, Math.ceil(vvn.cargoVolume / CARGO_VOLUME_TO_TIME_FACTOR));
      const loadTime = Math.max(1, Math.ceil(vvn.cargoVolume / CARGO_VOLUME_TO_TIME_FACTOR));

      facts.push({
        vesselLabel,
        vvnId: vvn.id,
        arrivalTime,
        departureTime,
        unloadTime,
        loadTime,
      });
    });

    return facts;
  }

  private dateToTimeUnits(date: Date, baseTime: Date): number {
    const diffMs = date.getTime() - baseTime.getTime();
    const diffMinutes = diffMs / (1000 * 60);
    return Math.max(0, Math.floor(diffMinutes / 10));
  }

  private generateVesselFacts(facts: PrologVesselFact[]): string {
    return facts
      .map(
        (f) =>
          `vessel(${f.vesselLabel}, ${f.arrivalTime}, ${f.departureTime}, ${f.unloadTime}, ${f.loadTime}).`
      )
      .join('\n');
  }

  private async generatePrologScript(
    vesselFacts: PrologVesselFact[]
  ): Promise<string> {
    const algorithmContent = await fs.readFile(this.prologScriptPath, 'utf-8');
    const newVesselFacts = this.generateVesselFacts(vesselFacts);
    const vesselFactPattern = /(?:%?vessel\([^)]+\)\.\s*\n)+/g;
    
    const sequenceStart = algorithmContent.indexOf('sequence_temporization');
    if (sequenceStart > 0) {
      const beforeSequence = algorithmContent.substring(0, sequenceStart);
      const afterSequence = algorithmContent.substring(sequenceStart);
      const cleanedBefore = beforeSequence.replace(vesselFactPattern, '');
      return `${cleanedBefore}${newVesselFacts}\n\n${afterSequence}`;
    }

    return algorithmContent.replace(vesselFactPattern, `${newVesselFacts}\n\n`);
  }

  async executePrologScheduling(
    vesselFacts: PrologVesselFact[]
  ): Promise<PrologScheduleResult[]> {
    await fs.mkdir(this.tempDir, { recursive: true });

    const tempFile = path.join(
      this.tempDir,
      `scheduler_${Date.now()}.pl`
    );
    const prologScript = await this.generatePrologScript(vesselFacts);
    await fs.writeFile(tempFile, prologScript, 'utf-8');

    try {
      const { stdout, stderr } = await execAsync(
        `swipl -g "consult('${tempFile.replace(/\\/g, '/')}'), obtain_seq_shortest_delay(Seq, Delay), halt." -t "halt(1)."`
      );

      const results = this.parsePrologOutput(stdout, vesselFacts);

      await fs.unlink(tempFile);

      return results;
    } catch (error: any) {
      try {
        await fs.unlink(tempFile);
      } catch {}

      throw new Error(
        `Prolog execution failed: ${error.message}\n${error.stderr || ''}`
      );
    }
  }

  private parsePrologOutput(
    output: string,
    vesselFacts: PrologVesselFact[]
  ): PrologScheduleResult[] {
    const results: PrologScheduleResult[] = [];

    const sequenceMatch = output.match(/Better Sequence:\s*\[(.*?)\]/s);
    if (!sequenceMatch) {
      throw new Error('Could not parse Prolog output: sequence not found');
    }

    const sequenceStr = sequenceMatch[1];
    const tripletRegex = /\(([^,]+),(\d+),(\d+)\)/g;
    let match;

    while ((match = tripletRegex.exec(sequenceStr)) !== null) {
      const [, vesselLabel, unloadStart, loadEnd] = match;

      const vesselFact = vesselFacts.find((f) => f.vesselLabel === vesselLabel.trim());
      const loadEndTime = parseInt(loadEnd, 10);
      const delay = vesselFact
        ? Math.max(0, loadEndTime + 1 - vesselFact.departureTime)
        : 0;

      results.push({
        vesselLabel: vesselLabel.trim(),
        unloadStartTime: parseInt(unloadStart, 10),
        loadEndTime: loadEndTime,
        delay: delay,
        dockId: undefined,
        craneId: undefined,
        staffId: undefined,
        storageId: undefined,
        timeFactor: undefined,
      });
    }

    return results;
  }

  async checkPrologInstalled(): Promise<boolean> {
    try {
      await execAsync('swipl --version');
      return true;
    } catch {
      return false;
    }
  }
}

export default new PrologService();

