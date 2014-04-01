<Serializable()>
Public Class ServiceBusEventMessage
    Inherits EventPayloadBase

    Private Property _BatchNo() As Integer

    Public Sub New()
        MyBase.New(EventType.WindowsServiceRequest)
    End Sub

    Public Sub New(ByVal batchNo As Integer)
        Me.New()
        _BatchNo = batchNo
    End Sub


    Public Overrides Function ToString() As String
        Return String.Format("Event {0} Message", _BatchNo)
    End Function
End Class
