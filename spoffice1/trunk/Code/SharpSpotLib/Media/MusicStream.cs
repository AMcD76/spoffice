﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SharpSpotLib.Media
{
    public class MusicStream : Stream
    {
        #region fields

        private Byte[] _buffer = new Byte[0];
        private Int32 _readPosition = 0;
        private Int32 _writePosition = 0;
        private Boolean _allDataAvailable = false;

        #endregion


        #region event

        public event EventHandler<EventArgs> NewDataAvailable;
        public event EventHandler<EventArgs> AllDataAvailable;

        #endregion


        #region properties

        public Boolean AllAvailable
        {
            get
            {
                return _allDataAvailable;
            }
            internal set
            {
                _allDataAvailable = value;
                if (value)
                {
                    try
                    {
                        var allAvailableEvent = AllDataAvailable;
                        if (allAvailableEvent != null)
                            allAvailableEvent(this, EventArgs.Empty);
                    }
                    catch (Exception) { }
                }
            }
        }

        public Int32 AvailableLength
        {
            get
            {
                if (_buffer == null)
                    throw new InvalidOperationException("Stream is closed");
                return _writePosition;
            }
        }

        public override bool CanRead
        {
            get 
            {
                if (_buffer == null)
                    return false;
                return true; 
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (_buffer == null)
                    return false;
                return true; 
            }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get
            {
                if (_buffer == null)
                    throw new InvalidOperationException("Stream is closed");
                return _buffer.Length; 
            }
        }

        public override long Position
        {
            get
            {
                if (_buffer == null)
                    throw new InvalidOperationException("Stream is closed");
                return _readPosition;
            }
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }

        #endregion


        #region methods

        public override void SetLength(long value)
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            if (value > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("value");
            SetBufferSize((Int32)value);
        }

        public override void Close()
        {
            _buffer = null;
        }

        public Byte[] GetBuffer()
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            return _buffer;
        }

        private void SetBufferSize(Int32 newSize)
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            if (_buffer.Length > 0)
            {
                Byte[] newBuffer = new Byte[newSize];
                Int32 toCopy = newSize > _writePosition ? _writePosition : newSize;
                Buffer.BlockCopy(_buffer, 0, newBuffer, 0, toCopy);
                _buffer = newBuffer;
            }
            else
                _buffer = new Byte[newSize];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            Int32 toRead = count;
            if (_readPosition + count > _writePosition)
                toRead = _writePosition - _readPosition;
            if (toRead < 0)
                toRead = 0;
            Buffer.BlockCopy(_buffer, (Int32)_readPosition, buffer, offset, toRead);
            _readPosition += toRead;
            return toRead;
        }

        public override int ReadByte()
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            if (_readPosition >= _writePosition)
                throw new ArgumentOutOfRangeException("Requested data is not yet available");
            return _buffer[_readPosition++];
        }

        public override void Flush()
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        internal void WriteInternal(byte[] buffer, int offset, int count)
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            if (_writePosition + count > this.Length)
                SetBufferSize(_writePosition + count);
            Buffer.BlockCopy(buffer, offset, _buffer, _writePosition, count);
            _writePosition += count;

            try
            {
                var newDataEvent = NewDataAvailable;
                if (newDataEvent != null)
                    newDataEvent(this, EventArgs.Empty);
            }
            catch (Exception) { }
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            if (_buffer == null)
                throw new InvalidOperationException("Stream is closed");
            if (offset > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("offset");
            Int32 newPosition = _readPosition;
            if (loc == SeekOrigin.Begin)
                newPosition = (Int32)offset;
            else if (loc == SeekOrigin.Current)
                newPosition += (Int32)offset;
            else if (loc == SeekOrigin.End)
                newPosition = (Int32)(this.Length + offset);
            if (newPosition > _writePosition)
                throw new InvalidOperationException();
            _readPosition = newPosition;
            return _readPosition;
        }

        #endregion


        #region construction

        #endregion
    }
}
