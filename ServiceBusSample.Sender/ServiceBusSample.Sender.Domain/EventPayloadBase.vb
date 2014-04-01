

<Serializable()> _
Public MustInherit Class EventPayloadBase

#Region "Members"

    Private _eventType As EventType
    Private _enqueueDateTime As DateTime

#End Region

#Region "Constructors"

    Public Sub New(ByVal type As EventType)
        _eventType = type
        _enqueueDateTime = DateTime.Now
    End Sub

#End Region

#Region "Properties"

    Public Property EventType() As EventType
        Get
            Return _eventType
        End Get
        Set(ByVal value As EventType)
            _eventType = value
        End Set
    End Property

    Public Property EnqueueDateTime() As DateTime
        Get
            Return _enqueueDateTime
        End Get
        Set(ByVal value As DateTime)
            _enqueueDateTime = value
        End Set
    End Property
#End Region

#Region " Object Overrides "

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj IsNot Nothing AndAlso Me.GetType.IsAssignableFrom(obj.GetType) Then
            Dim objTyped As EventPayloadBase = DirectCast(obj, EventPayloadBase)
            Return Me.EventType.Equals(objTyped.EventType)
        Else
            Return MyBase.Equals(obj)
        End If
    End Function

#End Region


End Class
