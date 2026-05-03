using FlyGon.Notifications;
using FundTransfer.Domain.Bus.Publishers;
using FundTransfer.Domain.Commands;
using FundTransfer.Domain.Entities;
using FundTransfer.Domain.Enums;
using FundTransfer.Domain.Handlers;
using FundTransfer.Domain.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FundTransfer.Domain.Test.Handlers
{
    partial class FundTransferHandlerStatusTransferCommandTests
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
        [TestCase(null)]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        [Category("/Handlers/FundTransferHandler/StatusTransferCommand")]
        public async Task ValidatingWrongCommandsStatusTransferCommand(Guid? transactionId)
        {
            var wrong = new StatusTransferCommand(transactionId);
            var commandResult = await _handler.Handle(wrong, default);
            Assert.That(commandResult.Sucess, Is.False);
            Assert.That(((IReadOnlyCollection<Notification>)commandResult.Data).Count, Is.EqualTo(1));
        }

        [Test]
        [TestCase("a4d4edbb-f9a9-4a59-9398-ed24ecb0dbfc")]
        [Category("/Handlers/FundTransferHandler/StatusTransferCommand")]
        public async Task ValidatingRightCommandsStatusTransferCommandWithErrorStatus(Guid? transactionId)
        {
            var transferRepository = new Mock<ITransferRepository>();
            transferRepository
                .Setup(x => x.GetAsync((Guid)transactionId, default))
                .Returns(Task.FromResult((Transfer)null));

            var bus = new Mock<IBusPublisher>();
            _handler = new FundTransferHandler(transferRepository.Object, bus.Object);
            var right = new StatusTransferCommand(transactionId);
            var commandResult = await _handler.Handle(right, default);
            Assert.That(commandResult.Sucess, Is.False);
            Assert.That(commandResult.Message, Is.EqualTo("Invalid transaction number"));
            Assert.That((TransferStatusEnum)commandResult.Data, Is.EqualTo(TransferStatusEnum.Error));
        }

        [Test]
        [TestCase("a4d4edbb-f9a9-4a59-9398-ed24ecb0dbfc")]
        [Category("/Handlers/FundTransferHandler/StatusTransferCommand")]
        public async Task ValidatingRightCommandsStatusTransferCommandWithInQueueStatus(Guid? transactionId)
        {
            var transferRepository = new Mock<ITransferRepository>();
            transferRepository
                .Setup(x => x.GetAsync((Guid)transactionId, default))
                .Returns(Task.FromResult(
                    new Transfer((Guid)transactionId, TransferStatusEnum.InQueue)));

            var bus = new Mock<IBusPublisher>();
            _handler = new FundTransferHandler(transferRepository.Object, bus.Object);
            var right = new StatusTransferCommand(transactionId);
            var commandResult = await _handler.Handle(right, default);
            Assert.That(commandResult.Sucess, Is.True);
            Assert.That(commandResult.Message, Is.EqualTo(""));
            Assert.That((TransferStatusEnum)commandResult.Data, Is.EqualTo(TransferStatusEnum.InQueue));
        }

        [Test]
        [TestCase("a4d4edbb-f9a9-4a59-9398-ed24ecb0dbfc")]
        [Category("/Handlers/FundTransferHandler/StatusTransferCommand")]
        public async Task ValidatingRightCommandsStatusTransferCommandWithException(Guid? transactionId)
        {
            var message = "Ocorreu um erro ao consultar no banco de dados";
            var transferRepository = new Mock<ITransferRepository>();
            transferRepository
                .Setup(x => x.GetAsync((Guid)transactionId, default))
                .Throws(new Exception(message));

            var bus = new Mock<IBusPublisher>();
            _handler = new FundTransferHandler(transferRepository.Object, bus.Object);
            var right = new StatusTransferCommand(transactionId);
            var commandResult = await _handler.Handle(right, default);
            Assert.That(commandResult.Sucess, Is.False);
            Assert.That(commandResult.Message, Is.EqualTo(message));
        }
    }
}