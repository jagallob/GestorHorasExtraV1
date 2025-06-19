using System.Threading.Tasks;
using ExtraHours.API.Controller;
using ExtraHours.API.Dto;
using ExtraHours.API.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace ExtraHours.API.Tests
{
    public class IngresoAutorizacionControllerTests
    {
        private readonly IEmailService _emailService;
        private readonly IngresoAutorizacionController _controller;

        public IngresoAutorizacionControllerTests()
        {
            _emailService = Substitute.For<IEmailService>();
            _controller = new IngresoAutorizacionController(_emailService);
        }

        [Fact]
        public async Task AutorizarIngreso_ValidDto_SendsEmailsAndReturnsOk()
        {
            // Arrange
            var dto = new IngresoAutorizacionDto
            {
                EmployeeName = "Juan Pérez",
                Date = "2025-06-22",
                EstimatedEntryTime = "08:00",
                EstimatedExitTime = "17:00",
                TaskDescription = "Mantenimiento de servidores",
                ManagerName = "Ana Gómez",
                ManagerEmail = "ana.gomez@empresa.com"
            };

            // Act
            var result = await _controller.AutorizarIngreso(dto);

            // Assert
            await _emailService.Received(1).SendEmailAsync(Arg.Is<string>(x => x.Contains("seguridad")), Arg.Any<string>(), Arg.Any<string>());
            await _emailService.Received(1).SendEmailAsync(Arg.Is<string>(x => x.Contains("monitoreo")), Arg.Any<string>(), Arg.Any<string>());
            await _emailService.Received(1).SendEmailAsync(Arg.Is<string>(x => x.Contains("talentohumano")), Arg.Any<string>(), Arg.Any<string>());
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
