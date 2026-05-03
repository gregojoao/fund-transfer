using FundTransfer.Domain.Commands;
using NUnit.Framework;

namespace FundTransfer.Domain.Test.Commands
{
    class TransferCommandTests
    {
        [Test]
        [TestCase("", "", 0f)]
        [TestCase("", "", -0.01f)]
        [TestCase(null, null, null)]
        [Category("/Commands/TransferCommand")]
        public void ValidatingWrongCommand(string accountOrigin, string accountDestination, float? value)
        {
            var wrong = new TransferCommand(accountOrigin, accountDestination, value);
            wrong.Validate();
            Assert.That(wrong.IsInvalid, Is.True);
            Assert.That(wrong.Notifications.Count, Is.EqualTo(3));
        }

        [Test]
        [TestCase("123", "321", 0.01f)]
        [Category("/Commands/TransferCommand")]
        public void ValidatingRightCommand(string accountOrigin, string accountDestination, float? value)
        {
            var right = new TransferCommand(accountOrigin, accountDestination, value);
            right.Validate();
            Assert.That(right.IsValid, Is.True);
            Assert.That(right.Notifications.Count, Is.EqualTo(0));
        }
    }
}