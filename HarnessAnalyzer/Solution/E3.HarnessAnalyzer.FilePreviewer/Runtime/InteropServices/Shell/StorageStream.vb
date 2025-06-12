﻿#If NETFRAMEWORK Then
Namespace Global.System.Runtime.InteropServices.Shell

    ''' <summary>
    ''' A wrapper for the native IStream object.
    ''' </summary>
    Friend Class StorageStream
        Inherits IO.Stream
        Implements IDisposable
        Private _stream As ComTypes.IStream
        Private _isReadOnly As Boolean = False

        Friend Sub New(stream As ComTypes.IStream, [readOnly] As Boolean)
            If stream Is Nothing Then
                Throw New ArgumentNullException("stream")
            End If
            _isReadOnly = [readOnly]
            _stream = stream
        End Sub

        ''' <summary>
        ''' Reads a single byte from the stream, moving the current position ahead by 1.
        ''' </summary>
        ''' <returns>A single byte from the stream, -1 if end of stream.</returns>
        Public Overrides Function ReadByte() As Integer
            ThrowIfDisposed()

            Dim buffer = New Byte(0) {}
            If Read(buffer, 0, 1) > 0 Then
                Return buffer(0)
            End If
            Return -1
        End Function

        ''' <summary>
        ''' Writes a single byte to the stream
        ''' </summary>
        ''' <param name="value">Byte to write to stream</param>
        Public Overrides Sub WriteByte(value As Byte)
            ThrowIfDisposed()
            Dim buffer = New Byte() {value}
            Write(buffer, 0, 1)
        End Sub

        ''' <summary>
        ''' Gets whether the stream can be read from.
        ''' </summary>
        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return _stream IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets whether seeking is supported by the stream.
        ''' </summary>
        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return _stream IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets whether the stream can be written to.
        ''' Always false.
        ''' </summary>
        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return _stream IsNot Nothing AndAlso Not _isReadOnly
            End Get
        End Property

        ''' <summary>
        ''' Reads a buffer worth of bytes from the stream.
        ''' </summary>
        ''' <param name="buffer">Buffer to fill</param>
        ''' <param name="offset">Offset to start filling in the buffer</param>
        ''' <param name="count">Number of bytes to read from the stream</param>
        ''' <returns></returns>
        Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
            ThrowIfDisposed()

            If buffer Is Nothing Then
                Throw New ArgumentNullException("buffer")
            End If
            If offset < 0 Then
                Throw New ArgumentOutOfRangeException("offset", "StorageStreamOffsetLessThanZero")
            End If
            If count < 0 Then
                Throw New ArgumentOutOfRangeException("count", "StorageStreamCountLessThanZero")
            End If
            If offset + count > buffer.Length Then
                Throw New ArgumentException("StorageStreamBufferOverflow", "count")
            End If

            Dim bytesRead = 0
            If count > 0 Then

                Dim ptr As System.IntPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(Marshal.SizeOf(Of ULong))
                Try
                    If offset = 0 Then
                        _stream.Read(buffer, count, ptr)
                        bytesRead = CInt(Marshal.ReadInt64(ptr))
                    Else
                        Dim tempBuffer = New Byte(count - 1) {}
                        _stream.Read(tempBuffer, count, ptr)

                        bytesRead = CInt(Marshal.ReadInt64(ptr))
                        If bytesRead > 0 Then
                            Array.Copy(tempBuffer, 0, buffer, offset, bytesRead)
                        End If
                    End If

                Finally
                    Marshal.FreeCoTaskMem(ptr)
                End Try
            End If
            Return bytesRead
        End Function

        ''' <summary>
        ''' Writes a buffer to the stream if able to do so.
        ''' </summary>
        ''' <param name="buffer">Buffer to write</param>
        ''' <param name="offset">Offset in buffer to start writing</param>
        ''' <param name="count">Number of bytes to write to the stream</param>
        Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
            ThrowIfDisposed()

            If _isReadOnly Then
                Throw New InvalidOperationException("Storage Stream is readonly!")
            End If
            If buffer Is Nothing Then
                Throw New ArgumentNullException("buffer")
            End If
            If offset < 0 Then
                Throw New ArgumentOutOfRangeException("offset", "StorageStreamOffsetLessThanZero")
            End If
            If count < 0 Then
                Throw New ArgumentOutOfRangeException("count", "StorageStreamCountLessThanZero")
            End If
            If offset + count > buffer.Length Then
                Throw New ArgumentException("StorageStreamBufferOverflow", "count")
            End If

            If count > 0 Then
                Dim ptr As System.IntPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(Marshal.SizeOf(Of ULong))
                Try
                    If offset = 0 Then
                        _stream.Write(buffer, count, ptr)
                    Else
                        Dim tempBuffer = New Byte(count - 1) {}
                        Array.Copy(buffer, offset, tempBuffer, 0, count)
                        _stream.Write(tempBuffer, count, ptr)
                    End If

                Finally
                    Marshal.FreeCoTaskMem(ptr)
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Gets the length of the IStream
        ''' </summary>
        Public Overrides ReadOnly Property Length As Long
            Get
                ThrowIfDisposed()
                Const STATFLAG_NONAME = 1
                Dim stats As ComTypes.STATSTG = Nothing
                _stream.Stat(stats, STATFLAG_NONAME)
                Return stats.cbSize
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the current position within the underlying IStream.
        ''' </summary>
        Public Overrides Property Position As Long
            Get
                ThrowIfDisposed()
                Return Seek(0, IO.SeekOrigin.Current)
            End Get
            Set(value As Long)
                ThrowIfDisposed()
                Seek(value, IO.SeekOrigin.Begin)
            End Set
        End Property

        ''' <summary>
        ''' Seeks within the underlying IStream.
        ''' </summary>
        ''' <param name="offset">Offset</param>
        ''' <param name="origin">Where to start seeking</param>
        ''' <returns></returns>
        Public Overrides Function Seek(offset As Long, origin As IO.SeekOrigin) As Long
            ThrowIfDisposed()
            Dim ptr As System.IntPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(Marshal.SizeOf(Of Long))
            Try
                If _stream IsNot Nothing Then
                    _stream.Seek(offset, origin, ptr)
                End If
                Return Marshal.ReadInt64(ptr)
            Finally
                Marshal.FreeCoTaskMem(ptr)
            End Try
        End Function

        ''' <summary>
        ''' Sets the length of the stream
        ''' </summary>
        ''' <param name="value"></param>
        Public Overrides Sub SetLength(value As Long)
            ThrowIfDisposed()
            If _stream IsNot Nothing Then
                _stream.SetSize(value)
            End If
        End Sub

        ''' <summary>
        ''' Commits data to be written to the stream if it is being cached.
        ''' </summary>
        Public Overrides Sub Flush()
            If _stream IsNot Nothing Then
                _stream.Commit(CInt(StorageStreamCommitOptions.None))
            End If
        End Sub

        ''' <summary>
        ''' Disposes the stream.
        ''' </summary>
        ''' <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            _stream = Nothing
            MyBase.Dispose(disposing)
        End Sub

        Private Sub ThrowIfDisposed()
            If _stream Is Nothing Then Throw New ObjectDisposedException([GetType]().Name)
        End Sub
    End Class
End Namespace

#End If