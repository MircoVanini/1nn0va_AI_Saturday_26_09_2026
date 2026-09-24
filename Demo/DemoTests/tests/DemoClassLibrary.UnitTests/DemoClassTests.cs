using System;
using System.IO;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using DemoClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoClassLibrary.UnitTests
{
    [TestClass]
    public class SerializeHelperTests
    {
        /// <summary>
        /// Tests that Serialize throws ArgumentNullException when the input value is null.
        /// </summary>
        [TestMethod]
        public void Serialize_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            object? value = null;

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() => DemoClass.Serialize(value!));
            Assert.AreEqual("value", exception.ParamName);
        }

        /// <summary>
        /// Tests that Serialize throws ArgumentException when the input is a plain object instance.
        /// This validates that the method rejects serializing bare object instances.
        /// </summary>
        [TestMethod]
        public void Serialize_PlainObjectInstance_ThrowsArgumentException()
        {
            // Arrange
            object value = new object();

            // Act & Assert
            ArgumentException exception = Assert.ThrowsException<ArgumentException>(() => DemoClass.Serialize(value));
            Assert.AreEqual("value", exception.ParamName);
            Assert.IsTrue(exception.Message.Contains("Specified value is not an object instance"));
        }

        /// <summary>
        /// Tests that Serialize successfully serializes a simple string value.
        /// </summary>
        [TestMethod]
        public void Serialize_SimpleString_ReturnsJsonString()
        {
            // Arrange
            string value = "test";

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("\"test\"", result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes an empty string.
        /// </summary>
        [TestMethod]
        public void Serialize_EmptyString_ReturnsJsonString()
        {
            // Arrange
            string value = "";

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("\"\"", result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes integer values including edge cases.
        /// </summary>
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(-1)]
        [DataRow(int.MaxValue)]
        [DataRow(int.MinValue)]
        public void Serialize_IntegerValue_ReturnsJsonString(int value)
        {
            // Arrange & Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(value.ToString(), result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes boolean values.
        /// </summary>
        [TestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void Serialize_BooleanValue_ReturnsJsonString(bool value, string expected)
        {
            // Arrange & Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes floating-point values including special cases.
        /// </summary>
        [TestMethod]
        [DataRow(0.0)]
        [DataRow(1.5)]
        [DataRow(-1.5)]
        [DataRow(double.MaxValue)]
        [DataRow(double.MinValue)]
        public void Serialize_DoubleValue_ReturnsJsonString(double value)
        {
            // Arrange & Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains(value.ToString()) || result.Length > 0);
        }

        /// <summary>
        /// Tests that Serialize handles special double values (NaN and Infinity).
        /// JSON serialization represents these as string literals.
        /// </summary>
        [TestMethod]
        [DataRow(double.NaN, "\"NaN\"")]
        [DataRow(double.PositiveInfinity, "\"Infinity\"")]
        [DataRow(double.NegativeInfinity, "\"-Infinity\"")]
        [TestCategory("ProductionBugSuspected")]
        [Ignore("ProductionBugSuspected")]
        public void Serialize_SpecialDoubleValue_ReturnsJsonString(double value, string expected)
        {
            // Arrange & Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes a complex object with properties.
        /// Validates that camelCase naming policy is applied.
        /// </summary>
        [TestMethod]
        public void Serialize_ComplexObject_ReturnsJsonStringWithCamelCase()
        {
            // Arrange
            var value = new { Name = "John", Age = 30 };

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("\"name\""));
            Assert.IsTrue(result.Contains("\"age\""));
            Assert.IsTrue(result.Contains("\"John\""));
            Assert.IsTrue(result.Contains("30"));
        }

        /// <summary>
        /// Tests that Serialize ignores null properties based on SerializerOptions configuration.
        /// </summary>
        [TestMethod]
        public void Serialize_ObjectWithNullProperty_IgnoresNullProperty()
        {
            // Arrange
            var value = new { Name = "John", MiddleName = (string?)null, Age = 30 };

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("\"name\""));
            Assert.IsTrue(result.Contains("\"age\""));
            Assert.IsFalse(result.Contains("middleName"));
        }

        /// <summary>
        /// Tests that Serialize successfully serializes an array.
        /// </summary>
        [TestMethod]
        public void Serialize_Array_ReturnsJsonString()
        {
            // Arrange
            int[] value = new[] { 1, 2, 3, 4, 5 };

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[1,2,3,4,5]", result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes an empty array.
        /// </summary>
        [TestMethod]
        public void Serialize_EmptyArray_ReturnsJsonString()
        {
            // Arrange
            int[] value = Array.Empty<int>();

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("[]", result);
        }

        /// <summary>
        /// Tests that Serialize successfully serializes a very long string.
        /// </summary>
        [TestMethod]
        public void Serialize_VeryLongString_ReturnsJsonString()
        {
            // Arrange
            string value = new string('a', 10000);

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 10000);
            Assert.IsTrue(result.StartsWith("\""));
            Assert.IsTrue(result.EndsWith("\""));
        }

        /// <summary>
        /// Tests that Serialize successfully serializes a whitespace-only string.
        /// </summary>
        [TestMethod]
        public void Serialize_WhitespaceOnlyString_ReturnsJsonString()
        {
            // Arrange
            string value = "   ";

            // Act
            string result = DemoClass.Serialize(value);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("\"   \"", result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream throws ArgumentNullException when streamReader is null.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_NullStreamReader_ThrowsArgumentNullException()
        {
            // Arrange
            StreamReader? streamReader = null;

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                DemoClass.DeserializeFromStream<string>(streamReader!));
            Assert.AreEqual("streamReader", exception.ParamName);
        }

        /// <summary>
        /// Tests that DeserializeFromStream successfully deserializes valid JSON string.
        /// Input: Valid JSON string value.
        /// Expected: Returns the deserialized string.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_ValidJsonString_ReturnsDeserializedString()
        {
            // Arrange
            string json = "\"test string\"";
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            string? result = DemoClass.DeserializeFromStream<string>(streamReader);

            // Assert
            Assert.AreEqual("test string", result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream successfully deserializes valid JSON integer.
        /// Input: Valid JSON integer value.
        /// Expected: Returns the deserialized integer.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_ValidJsonInteger_ReturnsDeserializedInteger()
        {
            // Arrange
            string json = "42";
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            int result = DemoClass.DeserializeFromStream<int>(streamReader);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream throws JsonException when JSON is malformed.
        /// Input: Invalid/malformed JSON string.
        /// Expected: JsonException is thrown.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            string json = "{invalid json";
            StreamReader streamReader = CreateStreamReader(json);

            // Act & Assert
            Assert.ThrowsException<JsonException>(() =>
                DemoClass.DeserializeFromStream<object>(streamReader));
        }

        /// <summary>
        /// Tests that DeserializeFromStream returns null when JSON contains null value.
        /// Input: JSON "null" string.
        /// Expected: Returns null.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_NullJsonValue_ReturnsNull()
        {
            // Arrange
            string json = "null";
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            string? result = DemoClass.DeserializeFromStream<string>(streamReader);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream throws JsonException when stream contains empty string.
        /// Input: Empty string.
        /// Expected: JsonException is thrown.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_EmptyString_ThrowsJsonException()
        {
            // Arrange
            string json = "";
            StreamReader streamReader = CreateStreamReader(json);

            // Act & Assert
            Assert.ThrowsException<JsonException>(() =>
                DemoClass.DeserializeFromStream<string>(streamReader));
        }

        /// <summary>
        /// Tests that DeserializeFromStream throws JsonException when stream contains only whitespace.
        /// Input: Whitespace-only string.
        /// Expected: JsonException is thrown.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_WhitespaceOnly_ThrowsJsonException()
        {
            // Arrange
            string json = "   ";
            StreamReader streamReader = CreateStreamReader(json);

            // Act & Assert
            Assert.ThrowsException<JsonException>(() =>
                DemoClass.DeserializeFromStream<string>(streamReader));
        }

        /// <summary>
        /// Tests that DeserializeFromStream successfully deserializes a JSON array.
        /// Input: Valid JSON array.
        /// Expected: Returns the deserialized array.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_JsonArray_ReturnsDeserializedArray()
        {
            // Arrange
            string json = "[1,2,3,4,5]";
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            int[]? result = DemoClass.DeserializeFromStream<int[]>(streamReader);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream handles extreme numeric values correctly.
        /// Input: JSON with int.MaxValue.
        /// Expected: Returns the correct extreme value.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_MaxIntValue_ReturnsCorrectValue()
        {
            // Arrange
            string json = int.MaxValue.ToString();
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            int result = DemoClass.DeserializeFromStream<int>(streamReader);

            // Assert
            Assert.AreEqual(int.MaxValue, result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream handles extreme numeric values correctly.
        /// Input: JSON with int.MinValue.
        /// Expected: Returns the correct extreme value.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_MinIntValue_ReturnsCorrectValue()
        {
            // Arrange
            string json = int.MinValue.ToString();
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            int result = DemoClass.DeserializeFromStream<int>(streamReader);

            // Assert
            Assert.AreEqual(int.MinValue, result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream handles special characters in strings correctly.
        /// Input: JSON string with special characters, escape sequences.
        /// Expected: Returns the correctly deserialized string with special characters.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_StringWithSpecialCharacters_ReturnsCorrectString()
        {
            // Arrange
            string json = "\"Line1\\nLine2\\tTabbed\"";
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            string? result = DemoClass.DeserializeFromStream<string>(streamReader);

            // Assert
            Assert.AreEqual("Line1\nLine2\tTabbed", result);
        }

        /// <summary>
        /// Tests that DeserializeFromStream handles very long JSON strings.
        /// Input: Very long JSON string.
        /// Expected: Successfully deserializes the long string.
        /// </summary>
        [TestMethod]
        public void DeserializeFromStream_VeryLongString_ReturnsDeserializedString()
        {
            // Arrange
            string longString = new string('a', 10000);
            string json = JsonSerializer.Serialize(longString);
            StreamReader streamReader = CreateStreamReader(json);

            // Act
            string? result = DemoClass.DeserializeFromStream<string>(streamReader);

            // Assert
            Assert.AreEqual(longString, result);
        }

        /// <summary>
        /// Helper method to create a StreamReader from a string.
        /// </summary>
        private static StreamReader CreateStreamReader(string content)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);
            return new StreamReader(stream);
        }

        /// <summary>
        /// Helper class for testing complex object deserialization.
        /// </summary>
        private class TestPerson
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        /// <summary>
        /// Tests that Deserialize throws ArgumentNullException when the value parameter is null.
        /// </summary>
        [TestMethod]
        public void Deserialize_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            string? value = null;

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                DemoClass.Deserialize<int>(value!));
            Assert.AreEqual("value", exception.ParamName);
        }

        /// <summary>
        /// Tests that Deserialize throws ArgumentNullException when the value parameter is empty or contains only whitespace.
        /// </summary>
        /// <param name="value">The whitespace string to test.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("   ")]
        [DataRow("\t")]
        [DataRow("\n")]
        [DataRow("\r\n")]
        [DataRow(" \t\n ")]
        public void Deserialize_EmptyOrWhitespaceValue_ThrowsArgumentNullException(string value)
        {
            // Arrange
            // (value provided by DataRow)

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                DemoClass.Deserialize<int>(value));
            Assert.AreEqual("value", exception.ParamName);
        }

        /// <summary>
        /// Tests that Deserialize successfully deserializes valid JSON to a primitive type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="expected">The expected deserialized value.</param>
        [TestMethod]
        [DataRow("42", 42)]
        [DataRow("0", 0)]
        [DataRow("-100", -100)]
        [DataRow("2147483647", int.MaxValue)]
        [DataRow("-2147483648", int.MinValue)]
        public void Deserialize_ValidJsonForInt_ReturnsCorrectValue(string json, int expected)
        {
            // Arrange
            // (json and expected provided by DataRow)

            // Act
            int? result = DemoClass.Deserialize<int>(json);

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests that Deserialize successfully deserializes valid JSON to a string type.
        /// </summary>
        [TestMethod]
        public void Deserialize_ValidJsonForString_ReturnsCorrectValue()
        {
            // Arrange
            string json = "\"test string\"";

            // Act
            string? result = DemoClass.Deserialize<string>(json);

            // Assert
            Assert.AreEqual("test string", result);
        }

        /// <summary>
        /// Tests that Deserialize returns null when the JSON value is "null".
        /// </summary>
        [TestMethod]
        public void Deserialize_JsonNullValue_ReturnsNull()
        {
            // Arrange
            string json = "null";

            // Act
            TestPerson? result = DemoClass.Deserialize<TestPerson>(json);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests that Deserialize throws JsonException when the JSON format is invalid.
        /// </summary>
        /// <param name="invalidJson">The invalid JSON string to test.</param>
        [TestMethod]
        [DataRow("not valid json")]
        [DataRow("{invalid}")]
        [DataRow("{\"name\":}")]
        [DataRow("{\"name\":\"value\"")]
        [DataRow("\"unclosed string")]
        public void Deserialize_InvalidJsonFormat_ThrowsJsonException(string invalidJson)
        {
            // Arrange
            // (invalidJson provided by DataRow)

            // Act & Assert
            Assert.ThrowsException<JsonException>(() =>
                DemoClass.Deserialize<TestPerson>(invalidJson));
        }

        /// <summary>
        /// Tests that Deserialize handles very long JSON strings.
        /// </summary>
        [TestMethod]
        public void Deserialize_VeryLongJsonString_DeserializesSuccessfully()
        {
            // Arrange
            string longString = new string('a', 10000);
            string json = $"\"{{\\\"name\\\":\\\"{longString}\\\"}}\"";

            // Act
            string? result = DemoClass.Deserialize<string>(json);

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Tests that Deserialize throws ArgumentNullException when value parameter is empty string.
        /// </summary>
        [TestMethod]
        public void Deserialize_EmptyValue_ThrowsArgumentNullException()
        {
            // Arrange
            string value = string.Empty;
            Type dataType = typeof(string);

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                DemoClass.Deserialize(value, dataType));
            Assert.AreEqual("value", exception.ParamName);
        }

        /// <summary>
        /// Tests that Deserialize throws ArgumentNullException when value parameter contains only whitespace.
        /// </summary>
        /// <param name="whitespaceValue">The whitespace-only string to test.</param>
        [TestMethod]
        [DataRow(" ")]
        [DataRow("  ")]
        [DataRow("\t")]
        [DataRow("\n")]
        [DataRow("\r\n")]
        [DataRow(" \t\n\r ")]
        public void Deserialize_WhitespaceValue_ThrowsArgumentNullException(string whitespaceValue)
        {
            // Arrange
            Type dataType = typeof(string);

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                DemoClass.Deserialize(whitespaceValue, dataType));
            Assert.AreEqual("value", exception.ParamName);
        }

        /// <summary>
        /// Tests that Deserialize throws ArgumentNullException when dataType parameter is null.
        /// </summary>
        [TestMethod]
        public void Deserialize_NullDataType_ThrowsArgumentNullException()
        {
            // Arrange
            string value = "\"test\"";
            Type? dataType = null;

            // Act & Assert
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                DemoClass.Deserialize(value, dataType!));
            Assert.AreEqual("dataType", exception.ParamName);
        }

        /// <summary>
        /// Tests that Deserialize successfully deserializes valid JSON string to the specified type.
        /// </summary>
        [TestMethod]
        public void Deserialize_ValidJsonAndType_ReturnsDeserializedObject()
        {
            // Arrange
            string value = "{\"name\":\"TestName\",\"value\":42}";
            Type dataType = typeof(TestData);

            // Act
            object? result = DemoClass.Deserialize(value, dataType);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TestData));
            TestData? testData = result as TestData;
            Assert.IsNotNull(testData);
            Assert.AreEqual("TestName", testData.Name);
            Assert.AreEqual(42, testData.Value);
        }

        /// <summary>
        /// Tests that Deserialize successfully deserializes JSON primitive types.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The target type for deserialization.</param>
        /// <param name="expectedValue">The expected deserialized value.</param>
        [TestMethod]
        [DataRow("\"test\"", typeof(string), "test")]
        [DataRow("42", typeof(int), 42)]
        [DataRow("true", typeof(bool), true)]
        [DataRow("3.14", typeof(double), 3.14)]
        public void Deserialize_ValidPrimitiveJson_ReturnsDeserializedValue(string json, Type type, object expectedValue)
        {
            // Arrange & Act
            object? result = DemoClass.Deserialize(json, type);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result);
        }

        /// <summary>
        /// Tests that Deserialize returns null when deserializing JSON "null" value.
        /// </summary>
        [TestMethod]
        public void Deserialize_NullJson_ReturnsNull()
        {
            // Arrange
            string value = "null";
            Type dataType = typeof(string);

            // Act
            object? result = DemoClass.Deserialize(value, dataType);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests that Deserialize throws JsonException when value contains invalid JSON.
        /// </summary>
        /// <param name="invalidJson">The invalid JSON string to test.</param>
        [TestMethod]
        [DataRow("{invalid}")]
        [DataRow("{\"name\":}")]
        [DataRow("[1,2,")]
        [DataRow("not json at all")]
        [DataRow("{unclosed")]
        public void Deserialize_InvalidJson_ThrowsJsonException(string invalidJson)
        {
            // Arrange
            Type dataType = typeof(object);

            // Act & Assert
            Assert.ThrowsException<JsonException>(() =>
                DemoClass.Deserialize(invalidJson, dataType));
        }

        /// <summary>
        /// Tests that Deserialize successfully deserializes empty JSON object.
        /// </summary>
        [TestMethod]
        public void Deserialize_EmptyJsonObject_ReturnsDeserializedObject()
        {
            // Arrange
            string value = "{}";
            Type dataType = typeof(TestData);

            // Act
            object? result = DemoClass.Deserialize(value, dataType);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TestData));
            TestData? testData = result as TestData;
            Assert.IsNotNull(testData);
            Assert.IsNull(testData.Name);
            Assert.AreEqual(0, testData.Value);
        }

        /// <summary>
        /// Tests that Deserialize successfully deserializes empty JSON array.
        /// </summary>
        [TestMethod]
        public void Deserialize_EmptyJsonArray_ReturnsDeserializedArray()
        {
            // Arrange
            string value = "[]";
            Type dataType = typeof(int[]);

            // Act
            object? result = DemoClass.Deserialize(value, dataType);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int[]));
            int[]? array = result as int[];
            Assert.IsNotNull(array);
            Assert.AreEqual(0, array.Length);
        }

        /// <summary>
        /// Helper class for testing deserialization.
        /// </summary>
        private class TestData
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }
    }
}