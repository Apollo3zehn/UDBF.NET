using System;
using System.Linq;
using Xunit;

namespace UDBF.NET.Tests
{
    public class GenericTests
    {
        [Fact]
        public void CanReadHeader() 
        {
            // Arrange
            var filePath = "testdata.dat";

            // Act
            using (var udbf = new UDBF(filePath))
            {
                // Assert
                Assert.True(udbf.ActTimeDataType == UDBFDataType.UnSignedInt64);
                Assert.True(udbf.ActTimeToSecondFactor == 1e-9);
                Assert.True(udbf.DataStartPosition == 160);
                Assert.True(udbf.HasTimeField == true);
                Assert.True(udbf.HeaderSize == 146);
                Assert.True(udbf.IsLittleEndian == true);
                Assert.True(udbf.ModuleAdditionalData != null);
                Assert.True(udbf.SampleRate == 25);
                Assert.True(udbf.StartTime == 36526);
                Assert.True(udbf.StartTimeToDayFactor == 1);
                Assert.True(udbf.TypeVendor == "UniversalDataBinFile - GANTNER instruments");
                Assert.True(udbf.Variables.Count == 2);
                Assert.True(udbf.Version == 107);
                Assert.True(udbf.WithCheckSum == true);

                Assert.True(udbf.Variables[0].AdditionalData != null);
                Assert.True(udbf.Variables[0].DataDirection == UDBFDataDirection.Input);
                Assert.True(udbf.Variables[0].DataType == UDBFDataType.Float);
                Assert.True(udbf.Variables[0].FieldLen == 8);
                Assert.True(udbf.Variables[0].Name == "WEA10_ACC_Y");
                Assert.True(udbf.Variables[0].Precision == 3);
                Assert.True(udbf.Variables[0].Unit == " V");
            }
        }

        [Fact]
        public void CanReadAll()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBF(filePath))
            {
                // Act
                (var timestamps, var dataset) = udbf.ReadAll();

                // Assert
                Assert.True(dataset.Count == udbf.Variables.Count);
                Assert.True(dataset[0].Buffer.Length == 15000);

                // Assert
                Assert.True(timestamps[0] == new DateTime(635853462000000000));
                Assert.True(timestamps[^1] == new DateTime(635853467999600000));

                Assert.True(dataset[0].Buffer.Length == 15000);
                Assert.True(dataset[0].Buffer[0] > 4.914855 && dataset[0].Buffer[0] < 4.914856);
                Assert.True(dataset[0].Buffer[^1] > 5.003571 && dataset[0].Buffer[^1] < 5.003572);

                Assert.True(dataset[1].Buffer.Length == 15000);
                Assert.True(dataset[1].Buffer[0] > 5.003258 && dataset[1].Buffer[0] < 5.003259);
                Assert.True(dataset[1].Buffer[^1] > 4.962193 && dataset[1].Buffer[^1] < 4.962194);
            }
        }

        [Fact]
        public void CanReadGeneric()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBF(filePath))
            {
                var variable1 = udbf.Variables[0];
                var variable2 = udbf.Variables[1];

                // Act
                (var timestamps1, var data1) = udbf.Read<float>(variable1);
                (var timestamps2, var data2) = udbf.Read<float>(variable2);

                // Assert
                Assert.True(timestamps1[0] == new DateTime(635853462000000000));
                Assert.True(timestamps1[^1] == new DateTime(635853467999600000));

                Assert.True(data1.Buffer.Length == 15000);
                Assert.True(data1.Buffer[0] > 4.914855 && data1.Buffer[0] < 4.914856);
                Assert.True(data1.Buffer[^1] > 5.003571 && data1.Buffer[^1] < 5.003572);

                Assert.True(data2.Buffer.Length == 15000);
                Assert.True(data2.Buffer[0] > 5.003258 && data2.Buffer[0] < 5.003259);
                Assert.True(data2.Buffer[^1] > 4.962193 && data2.Buffer[^1] < 4.962194);
            }
        }

        [Fact]
        public void ThrowsForExternalVariable()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBF(filePath))
            {
                var variable = new UDBFVariable();

                // Act / Assert
                Assert.Throws<ArgumentException>(() => udbf.Read<float>(variable));
            }
        }

        [Fact]
        public void ThrowsForInvalidDataTypeSize()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBF(filePath))
            {
                var variable = udbf.Variables.First();

                // Act / Assert
                Assert.Throws<InvalidOperationException>(() => udbf.Read<bool>(variable));
            }
        }
    }
}