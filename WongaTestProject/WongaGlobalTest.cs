namespace WongaTestProject
{
    public class WongaGlobalTest
    {

        #region IsValidMessage Tests
        /// <summary>
        /// Test with a valid string
        /// </summary>
        [Fact]
        public void ValidMessage()
        {
            // Arrange
            string testName = "Hello my name is, Ridgewell";

            // Act
            bool result = ServiceB.GetMessage.IsValidMessage(testName);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test with a invalid random  string
        /// </summary>
        [Fact]
        public void InvalidMessage()
        {
            // Arrange
            string testString = "This string is invalid";
            // Act
            bool result = ServiceB.GetMessage.IsValidMessage(testString);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test with a string where the name is null
        /// </summary>
        [Fact]
        public void NoNameMessage()
        {
            // Arrange
            string testString = "Hello my name is,";

            // Act
            bool result = ServiceB.GetMessage.IsValidMessage(testString);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test with message that has invalid name
        /// </summary>
        [Fact]
        public void InvalidNameMessage()
        {
            // Arrange
            string testString = "Hello my name is, Ridgewell423";

            // Act
            bool result = ServiceB.GetMessage.IsValidMessage(testString);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Get Name test

        /// <summary>
        /// Extracts name from valid string
        /// </summary>
        [Fact]
        public void ExtractGetNameSuccessfully()
        {
            // Arrange
            string testString = "Hello my name is, Ridgewell";

            // Act
            string name = ServiceB.GetMessage.GetName(testString);

            // Assert
            Assert.Equal("Ridgewell", name);
        }


        /// <summary>
        /// Extracts name from invalid string, throws exception
        /// </summary>
        [Fact]
        public void ExtractGetNameWithError()
        {
            // Arrange
            string testString = "Wonga String";

            // Act and Assert
            Exception ex = Assert.Throws<ArgumentException>(() => ServiceB.GetMessage.GetName(testString));

            // Assert
            Assert.Equal(ex.Message, testString);

        }
        #endregion
    }
}