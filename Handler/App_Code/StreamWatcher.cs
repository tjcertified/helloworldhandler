using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for StreamWatcher
/// </summary>
public class StreamWatcher : Stream
{
    private Stream _base;
    private MemoryStream _memoryStream = new MemoryStream();

    public StreamWatcher(Stream stream)
    {
        _base = stream;
    }

    public override void Flush()
    {
        _base.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _base.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _memoryStream.Write(buffer, offset, count);
        _base.Write(buffer, offset, count);
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(_memoryStream.ToArray());
    }

    #region Rest of the overrides
    public override bool CanRead
    {
        get { return _memoryStream.CanRead; }
    }

    public override bool CanSeek
    {
        get { return _memoryStream.CanSeek; }
    }

    public override bool CanWrite
    {
        get { return _memoryStream.CanWrite; }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _memoryStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _memoryStream.SetLength(value);
    }

    public override long Length
    {
        get { return _memoryStream.Length; }
    }

    public override long Position
    {
        get
        {
            return _memoryStream.Position;
        }
        set
        {
            _memoryStream.Position = value;
        }
    }
    #endregion
}