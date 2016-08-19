#region Copyright and License Notice
// Copyright (C)2010-2016 - INEX Solutions Ltd
// https://github.com/inex-solutions/configgen
// 
// This file is part of ConfigGen.
// 
// ConfigGen is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// ConfigGen is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License and 
// the GNU Lesser General Public License along with ConfigGen.  
// If not, see <http://www.gnu.org/licenses/>
#endregion

using System.IO;
using ConfigGen.Tests.Common;
using ConfigGen.Utilities.IO;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace ConfigGen.Utilities.Tests.IO.PauseableWritableStreamTests
{
    [Subject(typeof(PauseableWriteableStream))]
    public class when_decorating_an_underlying_stream : MachineSpecificationTestBase<PauseableWriteableStream>
    {
        private static Stream UnderlyingStream;
        private static Mock<Stream> MockUnderlyingStream;

        Establish context = () =>
        {
            MockUnderlyingStream = new Mock<Stream>(MockBehavior.Loose);

            MockUnderlyingStream.Setup(s => s.CanRead).Returns(true);
            MockUnderlyingStream.Setup(s => s.CanWrite).Returns(true);
            MockUnderlyingStream.Setup(s => s.CanSeek).Returns(true);
            MockUnderlyingStream.Setup(s => s.Length).Returns(5);
            MockUnderlyingStream.Setup(s => s.Position).Returns(6);

            UnderlyingStream = MockUnderlyingStream.Object;
        };

        Because of = () => Subject = new PauseableWriteableStream(UnderlyingStream);

        It the_value_of_CanRead_is_passed_through = () => Subject.CanRead.ShouldBeTrue();

        It the_value_of_CanWrite_is_passed_through = () => Subject.CanWrite.ShouldBeTrue();

        It the_value_of_CanSeek_is_passed_through = () => Subject.CanSeek.ShouldBeTrue();

        It calls_to_Flush_are_passed_through = () =>
        {
            Subject.Flush();
            MockUnderlyingStream.Verify(s => s.Flush());
        };

        It the_value_of_Length_is_passed_through = () => Subject.Length.ShouldEqual(5);

        It the_value_of_Position_is_passed_through = () => Subject.Position.ShouldEqual(6);

        It calls_to_set_Position_are_passed_through = () =>
        {
            Subject.Position = 7;
            MockUnderlyingStream.VerifySet(s => s.Position = 7);
        };

        It calls_to_Read_are_passed_through = () =>
        {
            var buffer = new byte[100];
            MockUnderlyingStream.Setup(s => s.Read(buffer, 5, 10)).Returns(8);
            int read = Subject.Read(buffer, 5, 10);

            MockUnderlyingStream.Verify(s => s.Read(buffer, 5, 10));
            read.ShouldEqual(8);
        };

        It calls_to_Seek_are_passed_through = () =>
        {
            Subject.Seek(9, SeekOrigin.End);
            MockUnderlyingStream.Verify(s => s.Seek(9, SeekOrigin.End));
        };

        It calls_to_SetLength_are_passed_through = () =>
        {
            Subject.SetLength(11);
            MockUnderlyingStream.Verify(s => s.SetLength(11));
        };

        It calls_to_Write_are_passed_through = () =>
        {
            var buffer = new byte[100];
            Subject.Write(buffer, 6, 7);
            MockUnderlyingStream.Verify(s => s.Write(buffer, 6, 7));
        };
    }

    [Subject(typeof(PauseableWriteableStream))]
    public class when_PauseWriting_is_called_and_then_further_data_is_written_to_the_stream : MachineSpecificationTestBase<PauseableWriteableStream>
    {
        private static Stream UnderlyingStream;
        private static Mock<Stream> MockUnderlyingStream;

        Establish context = () =>
        {
            MockUnderlyingStream = new Mock<Stream>(MockBehavior.Loose);

            UnderlyingStream = MockUnderlyingStream.Object;

            Subject = new PauseableWriteableStream(UnderlyingStream);
        };

        Because of = () =>
        {
            Subject.PauseWriting();
            Subject.Write(new byte[] { 0x42, 0x44, 0x46 }, 1, 2);
        };

        It the_data_is_not_written_to_the_underlying_stream =
            () => MockUnderlyingStream.Verify(s => s.Write(Moq.It.IsAny<byte[]>(), Moq.It.IsAny<int>(), Moq.It.IsAny<int>()), Times.Never);
    }

    [Subject(typeof(PauseableWriteableStream))]
    public class when_PauseWriting_is_called_and_then_Resume_writing_is_called : MachineSpecificationTestBase<PauseableWriteableStream>
    {
        private static Stream UnderlyingStream;
        private static Mock<Stream> MockUnderlyingStream;

        Establish context = () =>
        {
            MockUnderlyingStream = new Mock<Stream>(MockBehavior.Loose);

            UnderlyingStream = MockUnderlyingStream.Object;

            Subject = new PauseableWriteableStream(UnderlyingStream);
        };

        Because of = () =>
        {

            Subject.PauseWriting();
            Subject.Write(new byte[] { 0x42, 0x44, 0x46 }, 1, 2);
            Subject.ResumeWriting();
            Subject.Write(new byte[] { 0x52, 0x54, 0x56 }, 0, 3);
        };

        It the_data_written_while_paused_is_not_subsequently_written_to_the_underlying_stream =
            () => MockUnderlyingStream.Verify(s => s.Write(new byte[] { 0x42, 0x44, 0x46 }, 1, 2), Times.Never);

        It the_data_written_once_resumed_is_subsequently_written_to_the_underlying_stream =
            () => MockUnderlyingStream.Verify(s => s.Write(new byte[] {0x52, 0x54, 0x56}, 0, 3), Times.Once);
    }
}