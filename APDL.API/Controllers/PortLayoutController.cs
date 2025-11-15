using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortLayoutController : ControllerBase
    {
        [HttpGet("{layoutId}")]
        public IActionResult GetPortLayout(string layoutId)
        {
            var layouts = new Dictionary<string, object>
            {
                ["layout1"] = new
                {
                    id = "layout1",
                    name = "Small Port - 3 Docks Left Side",
                    docks = new[]
                    {
                        new
                        {
                            id = "dock1",
                            position = new
                            {
                                x = -120,
                                y = 0,
                                z = -60,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 5,
                                depth = 25,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d1-1",
                                    position = new
                                    {
                                        x = -120,
                                        y = 0,
                                        z = -60,
                                    },
                                    height = 40,
                                },
                                new
                                {
                                    id = "crane-d1-2",
                                    position = new
                                    {
                                        x = -100,
                                        y = 0,
                                        z = -60,
                                    },
                                    height = 40,
                                },
                            },
                        },
                        new
                        {
                            id = "dock2",
                            position = new
                            {
                                x = -120,
                                y = 0,
                                z = 0,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 5,
                                depth = 25,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d2-1",
                                    position = new
                                    {
                                        x = -120,
                                        y = 0,
                                        z = 0,
                                    },
                                    height = 40,
                                },
                            },
                        },
                        new
                        {
                            id = "dock3",
                            position = new
                            {
                                x = -120,
                                y = 0,
                                z = 60,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 5,
                                depth = 25,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d3-1",
                                    position = new
                                    {
                                        x = -120,
                                        y = 0,
                                        z = 60,
                                    },
                                    height = 40,
                                },
                                new
                                {
                                    id = "crane-d3-2",
                                    position = new
                                    {
                                        x = -105,
                                        y = 0,
                                        z = 60,
                                    },
                                    height = 40,
                                },
                                new
                                {
                                    id = "crane-d3-3",
                                    position = new
                                    {
                                        x = -90,
                                        y = 0,
                                        z = 60,
                                    },
                                    height = 40,
                                },
                            },
                        },
                    },
                    containerYards = new[]
                    {
                        new
                        {
                            id = "yard1",
                            position = new
                            {
                                x = -20,
                                y = 0,
                                z = -40,
                            },
                            dimensions = new
                            {
                                width = 80,
                                height = 2,
                                depth = 60,
                            },
                            capacity = 500,
                        },
                        new
                        {
                            id = "yard2",
                            position = new
                            {
                                x = -20,
                                y = 0,
                                z = 40,
                            },
                            dimensions = new
                            {
                                width = 70,
                                height = 2,
                                depth = 50,
                            },
                            capacity = 350,
                        },
                    },
                    warehouses = new[]
                    {
                        new
                        {
                            id = "warehouse1",
                            position = new
                            {
                                x = 80,
                                y = 0,
                                z = -50,
                            },
                            dimensions = new
                            {
                                width = 40,
                                height = 20,
                                depth = 30,
                            },
                        },
                        new
                        {
                            id = "warehouse2",
                            position = new
                            {
                                x = 80,
                                y = 0,
                                z = 10,
                            },
                            dimensions = new
                            {
                                width = 35,
                                height = 18,
                                depth = 25,
                            },
                        },
                        new
                        {
                            id = "warehouse3",
                            position = new
                            {
                                x = 80,
                                y = 0,
                                z = 60,
                            },
                            dimensions = new
                            {
                                width = 38,
                                height = 22,
                                depth = 28,
                            },
                        },
                    },
                },
                ["layout2"] = new
                {
                    id = "layout2",
                    name = "Large Port - 5 Docks Both Sides",
                    docks = new[]
                    {
                        new
                        {
                            id = "dock1",
                            position = new
                            {
                                x = -140,
                                y = 0,
                                z = -80,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 5,
                                depth = 25,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d1-1",
                                    position = new
                                    {
                                        x = -140,
                                        y = 0,
                                        z = -80,
                                    },
                                    height = 40,
                                },
                            },
                        },
                        new
                        {
                            id = "dock2",
                            position = new
                            {
                                x = -140,
                                y = 0,
                                z = -30,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 5,
                                depth = 25,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d2-1",
                                    position = new
                                    {
                                        x = -140,
                                        y = 0,
                                        z = -30,
                                    },
                                    height = 40,
                                },
                                new
                                {
                                    id = "crane-d2-2",
                                    position = new
                                    {
                                        x = -120,
                                        y = 0,
                                        z = -30,
                                    },
                                    height = 40,
                                },
                            },
                        },
                        new
                        {
                            id = "dock3",
                            position = new
                            {
                                x = -140,
                                y = 0,
                                z = 30,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 5,
                                depth = 25,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d3-1",
                                    position = new
                                    {
                                        x = -140,
                                        y = 0,
                                        z = 30,
                                    },
                                    height = 40,
                                },
                            },
                        },
                        new
                        {
                            id = "dock4",
                            position = new
                            {
                                x = 140,
                                y = 0,
                                z = -60,
                            },
                            dimensions = new
                            {
                                width = 80,
                                height = 5,
                                depth = 30,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d4-1",
                                    position = new
                                    {
                                        x = 140,
                                        y = 0,
                                        z = -60,
                                    },
                                    height = 45,
                                },
                                new
                                {
                                    id = "crane-d4-2",
                                    position = new
                                    {
                                        x = 160,
                                        y = 0,
                                        z = -60,
                                    },
                                    height = 45,
                                },
                                new
                                {
                                    id = "crane-d4-3",
                                    position = new
                                    {
                                        x = 180,
                                        y = 0,
                                        z = -60,
                                    },
                                    height = 45,
                                },
                            },
                        },
                        new
                        {
                            id = "dock5",
                            position = new
                            {
                                x = 140,
                                y = 0,
                                z = 20,
                            },
                            dimensions = new
                            {
                                width = 70,
                                height = 5,
                                depth = 28,
                            },
                            stsCranes = new[]
                            {
                                new
                                {
                                    id = "crane-d5-1",
                                    position = new
                                    {
                                        x = 140,
                                        y = 0,
                                        z = 20,
                                    },
                                    height = 45,
                                },
                                new
                                {
                                    id = "crane-d5-2",
                                    position = new
                                    {
                                        x = 160,
                                        y = 0,
                                        z = 20,
                                    },
                                    height = 45,
                                },
                            },
                        },
                    },
                    containerYards = new[]
                    {
                        new
                        {
                            id = "yard1",
                            position = new
                            {
                                x = -30,
                                y = 0,
                                z = -60,
                            },
                            dimensions = new
                            {
                                width = 90,
                                height = 2,
                                depth = 70,
                            },
                            capacity = 600,
                        },
                        new
                        {
                            id = "yard2",
                            position = new
                            {
                                x = 30,
                                y = 0,
                                z = 30,
                            },
                            dimensions = new
                            {
                                width = 85,
                                height = 2,
                                depth = 65,
                            },
                            capacity = 550,
                        },
                        new
                        {
                            id = "yard3",
                            position = new
                            {
                                x = -30,
                                y = 0,
                                z = 90,
                            },
                            dimensions = new
                            {
                                width = 60,
                                height = 2,
                                depth = 50,
                            },
                            capacity = 400,
                        },
                    },
                    warehouses = new[]
                    {
                        new
                        {
                            id = "warehouse1",
                            position = new
                            {
                                x = -150,
                                y = 0,
                                z = 90,
                            },
                            dimensions = new
                            {
                                width = 40,
                                height = 20,
                                depth = 30,
                            },
                        },
                        new
                        {
                            id = "warehouse2",
                            position = new
                            {
                                x = 140,
                                y = 0,
                                z = 80,
                            },
                            dimensions = new
                            {
                                width = 45,
                                height = 22,
                                depth = 35,
                            },
                        },
                        new
                        {
                            id = "warehouse3",
                            position = new
                            {
                                x = 0,
                                y = 0,
                                z = -120,
                            },
                            dimensions = new
                            {
                                width = 50,
                                height = 18,
                                depth = 28,
                            },
                        },
                        new
                        {
                            id = "warehouse4",
                            position = new
                            {
                                x = 60,
                                y = 0,
                                z = -120,
                            },
                            dimensions = new
                            {
                                width = 42,
                                height = 19,
                                depth = 26,
                            },
                        },
                    },
                },
            };

            if (!layouts.ContainsKey(layoutId))
            {
                return NotFound($"Layout '{layoutId}' not found");
            }

            return Ok(layouts[layoutId]);
        }

        [HttpGet]
        public IActionResult GetDefaultLayout()
        {
            return GetPortLayout("layout1");
        }
    }
}
