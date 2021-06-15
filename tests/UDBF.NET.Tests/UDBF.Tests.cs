using System;
using System.Linq;
using System.Runtime.InteropServices;
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
            using (var udbf = new UDBFFile(filePath))
            {
                // Assert
                Assert.Equal(UDBFDataType.UnSignedInt64, udbf.ActTimeDataType);
                Assert.Equal(1e-9, udbf.ActTimeToSecondFactor);
                Assert.Equal(160, udbf.DataStartPosition);
                Assert.True(udbf.HasTimeField);
                Assert.Equal(146, udbf.HeaderSize);
                Assert.True(udbf.IsLittleEndian);
                Assert.True(udbf.ModuleAdditionalData != null);
                Assert.Equal(25, udbf.SampleRate);
                Assert.Equal(36526, udbf.StartTime);
                Assert.Equal(1, udbf.StartTimeToDayFactor);
                Assert.Equal("UniversalDataBinFile - GANTNER instruments", udbf.TypeVendor);
                Assert.Equal(2, udbf.Variables.Count);
                Assert.Equal(107, udbf.Version);
                Assert.True(udbf.WithCheckSum);

                Assert.True(udbf.Variables[0].AdditionalData != null);
                Assert.Equal(UDBFDataDirection.Input, udbf.Variables[0].DataDirection);
                Assert.Equal(UDBFDataType.Float, udbf.Variables[0].DataType);
                Assert.Equal(8, udbf.Variables[0].FieldLen);
                Assert.Equal("WEA10_ACC_Y", udbf.Variables[0].Name);
                Assert.Equal(3, udbf.Variables[0].Precision);
                Assert.Equal(" V", udbf.Variables[0].Unit);
            }
        }

        [Fact]
        public void CanReadAll()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBFFile(filePath))
            {
                // Act
                (var timestamps, var dataset) = udbf.ReadAll();

                // Assert
                Assert.Equal(udbf.Variables.Count, dataset.Count);
                Assert.Equal(15000, dataset[0].Buffer.Length);

                // Assert
                Assert.Equal(new DateTime(635853462000000000), timestamps[0]);
                Assert.Equal(new DateTime(635853467999600000), timestamps[^1]);

                Assert.Equal(15000, dataset[0].Buffer.Length);
                Assert.Equal(4.914855, dataset[0].Buffer[0], precision: 6);
                Assert.Equal(5.003572, dataset[0].Buffer[^1], precision: 6);

                Assert.Equal(15000, dataset[1].Buffer.Length);
                Assert.Equal(5.003258, dataset[1].Buffer[0], precision: 6);
                Assert.Equal(4.962194, dataset[1].Buffer[^1], precision: 6);
            }
        }

        [Fact]
        public void CanReadGeneric()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBFFile(filePath))
            {
                var variable1 = udbf.Variables[0];
                var variable2 = udbf.Variables[1];

                // Act
                (var timestamps1, var data1) = udbf.Read<float>(variable1);
                (var timestamps2, var data2) = udbf.Read<float>(variable2);

                // Assert
                Assert.Equal(new DateTime(635853462000000000), timestamps1[0]);
                Assert.Equal(new DateTime(635853467999600000), timestamps1[^1]);

                Assert.Equal(15000, data1.Buffer.Length);
                Assert.Equal(4.914855, data1.Buffer[0], precision: 6);
                Assert.Equal(5.003572, data1.Buffer[^1], precision: 6);

                Assert.Equal(15000, data2.Buffer.Length);
                Assert.Equal(5.003258, data2.Buffer[0], precision: 6);
                Assert.Equal(4.962194, data2.Buffer[^1], precision: 6);
            }
        }

        [Fact]
        public void CanReadByteArray()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBFFile(filePath))
            {
                var variable1 = udbf.Variables[0];
                var variable2 = udbf.Variables[1];

                // Act
                (var timestamps1, var data1_raw) = udbf.Read<byte>(variable1);
                (var timestamps2, var data2_raw) = udbf.Read<byte>(variable2);

                // Assert
                var data1 = MemoryMarshal.Cast<byte, float>(data1_raw.Buffer);
                var data2 = MemoryMarshal.Cast<byte, float>(data2_raw.Buffer);

                Assert.Equal(new DateTime(635853462000000000), timestamps1[0]);
                Assert.Equal(new DateTime(635853467999600000), timestamps1[^1]);

                Assert.Equal(15000, data1.Length);
                Assert.Equal(4.914855, data1[0], precision: 6);
                Assert.Equal(5.003572, data1[^1], precision: 6);

                Assert.Equal(15000, data2.Length);
                Assert.Equal(5.003258, data2[0], precision: 6);
                Assert.Equal(4.962194, data2[^1], precision: 6);
            }
        }

        [Fact]
        public void ThrowsForExternalVariable()
        {
            // Arrange
            var filePath = "testdata.dat";

            using (var udbf = new UDBFFile(filePath))
            {
                var variable = new UDBFVariable();

                // Act / Assert
                Assert.Throws<ArgumentException>(() => udbf.Read<float>(variable));
            }
        }
    }
}