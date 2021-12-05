using System;
using Bot.Extensions;
using Xunit;

namespace Bot.Tests.Helpers;

public class DateTimeHelpersTests
{
    [Fact]
    public void GetLastSunday_ShouldProduceCorrectDate()
    {
        // Arrange

        var dateTime = new DateTime(2021, 06, 18, 1, 1, 1);

        // Act

        var lastSunday = dateTime.GetLastSunday();

        // Assert

        Assert.Equal(new(2021, 06, 13), lastSunday);
    }
}