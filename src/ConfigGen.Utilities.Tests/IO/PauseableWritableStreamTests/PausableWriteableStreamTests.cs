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
using ConfigGen.Tests.Common.Framework;
using ConfigGen.Utilities.IO;
using Moq;
using Shouldly;

// ReSharper disable PossibleNullReferenceException

namespace ConfigGen.Utilities.Tests.IO.PauseableWritableStreamTests
{
    public abstract class PauseableWriteableStreamTestBase : SpecificationTestBase<PauseableWriteableStream>
    {
        protected Stream UnderlyingStream;
        protected Mock<Stream> MockUnderlyingStream;
    }

    public class when_decorating_an_underlying_stream : PauseableWriteableStreamTestBase
    {
        public override void Given()
        {
            MockUnderlyingStream = new Mock<Stream>(MockBehavior.Loose);

            MockUnderlyingStream.Setup(s => s.CanRead).Returns(true);
            MockUnderlyingStream.Setup(s => s.CanWrite).Returns(true);
            MockUnderlyingStream.Setup(s => s.CanSeek).Returns(true);
            MockUnderlyingStream.Setup(s => s.Length).Returns(5);
            MockUnderlyingStream.Setup(s => s.Position).Returns(6);

            UnderlyingStream = MockUnderlyingStream.Object;
        }

        public override void When() => Subject = new PauseableWriteableStream(UnderlyingStream);

        [Then] public void the_value_of_CanRead_is_passed_through() => Subject.CanRead.ShouldBeTrue();

        [Then] public void the_value_of_CanWrite_is_passed_through() => Subject.CanWrite.ShouldBeTrue();

        [Then] public void the_value_of_CanSeek_is_passed_through() => Subject.CanSeek.ShouldBeTrue();

        [Then]
        public void calls_to_Flush_are_passed_through()
        {
            Subject.Flush();
            MockUnderlyingStream.Verify(s => s.Flush());
        }

        [Then]
        public void the_value_of_Length_is_passed_through() => Subject.Length.ShouldBe(5);

        [Then]
        public void the_value_of_Position_is_passed_through() => Subject.Position.ShouldBe(6);

        [Then]
        public void calls_to_set_Position_are_passed_through()
        {
            Subject.Position = 7;
            MockUnderlyingStream.VerifySet(s => s.Position = 7);
        }

        [Then]
        public void calls_to_Read_are_passed_through()
        {
            var buffer = new byte[100];
            MockUnderlyingStream.Setup(s => s.Read(buffer, 5, 10)).Returns(8);
            int read = Subject.Read(buffer, 5, 10);

            MockUnderlyingStream.Verify(s => s.Read(buffer, 5, 10));
            read.ShouldBe(8);
        }

        [Then]
        public void calls_to_Seek_are_passed_through()
        {
            Subject.Seek(9, SeekOrigin.End);
            MockUnderlyingStream.Verify(s => s.Seek(9, SeekOrigin.End));
        }

        [Then]
        public void calls_to_SetLength_are_passed_through()
        {
            Subject.SetLength(11);
            MockUnderlyingStream.Verify(s => s.SetLength(11));
        }

        [Then]
        public void calls_to_Write_are_passed_through()
        {
            var buffer = new byte[100];
            Subject.Write(buffer, 6, 7);
            MockUnderlyingStream.Verify(s => s.Write(buffer, 6, 7));
        }
    }

    
    public class when_PauseWriting_is_called_and_then_further_data_is_written_to_the_stream : PauseableWriteableStreamTestBase
    {
        public override void Given()
        {
            MockUnderlyingStream = new Mock<Stream>(MockBehavior.Loose);

            UnderlyingStream = MockUnderlyingStream.Object;

            Subject = new PauseableWriteableStream(UnderlyingStream);
        }

        public override void When()
        {
            Subject.PauseWriting();
            Subject.Write(new byte[] { 0x42, 0x44, 0x46 }, 1, 2);
        }

        [Then]
        public void the_data_is_not_written_to_the_underlying_stream() 
            => MockUnderlyingStream.Verify(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    
    public class when_PauseWriting_is_called_and_then_Resume_writing_is_called : PauseableWriteableStreamTestBase
    {
        public override void Given()
        {
            MockUnderlyingStream = new Mock<Stream>(MockBehavior.Loose);

            UnderlyingStream = MockUnderlyingStream.Object;

            Subject = new PauseableWriteableStream(UnderlyingStream);
        }

        public override void When()
        {

            Subject.PauseWriting();
            Subject.Write(new byte[] { 0x42, 0x44, 0x46 }, 1, 2);
            Subject.ResumeWriting();
            Subject.Write(new byte[] { 0x52, 0x54, 0x56 }, 0, 3);
        }

        [Then]
        public void the_data_written_while_paused_is_not_subsequently_written_to_the_underlying_stream()
            => MockUnderlyingStream.Verify(s => s.Write(new byte[] { 0x42, 0x44, 0x46 }, 1, 2), Times.Never);

        [Then]
        public void the_data_written_once_resumed_is_subsequently_written_to_the_underlying_stream() 
            => MockUnderlyingStream.Verify(s => s.Write(new byte[] {0x52, 0x54, 0x56}, 0, 3), Times.Once);
    }
}