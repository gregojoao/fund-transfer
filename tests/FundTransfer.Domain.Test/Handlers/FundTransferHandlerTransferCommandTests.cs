using FundTransfer.Domain.Bus.Publishers;
using FundTransfer.Domain.Commands;
using FundTransfer.Domain.Handlers;
using FundTransfer.Domain.Repositories;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FundTransfer.Domain.Test.Handlers
{
    partial class FundTransferHandlerTransferCommandTests
    {
        private FundTransferHandler _handler;

        [SetUp]
        public void Setup()
        {
            var transferRepository = new Mock<ITransferRepository>();
            var bus = new Mock<IBusPublisher>();
            _handler = new FundTransferHandler(transferRepository.Object, bus.Object);
        }

        [Test]
        [TestCase("", "", 0f)]
        [TestCase("", "", -0.01f)]
        [TestCase(null, null, null)]
        [Category("/Handlers/FundTransferHandler/TransferCommand")]
        public async Task ValidatingWrongCommandsTransferCommand(
            string accountOrigin, string accountDestination, float? value)
        {
            var wrong = new TransferCommand(accountOrigin, accountDestination, value);
            var commandResult = await _handler.Handle(wrong, default);
            Assert.That(commandResult.Sucess, Is.True);
            Assert.That(commandResult.Message, Is.EqualTo("Error"));
        }

        [Test]
        [TestCase("123", "321", 0.01f)]
        [Category("/Handlers/FundTransferHandler/TransferCommand")]
        public async Task ValidatingRightCommandsTransferCommand(
            string accountOrigin, string accountDestination, float? value)
        {
            var wrong = new TransferCommand(accountOrigin, accountDestination, value);
            var commandResult = await _handler.Handle(wrong, default);
            Assert.That(commandResult.Sucess, Is.True);
            Assert.That(commandResult.Message, Is.EqualTo("InQueue"));
        }
    }
}