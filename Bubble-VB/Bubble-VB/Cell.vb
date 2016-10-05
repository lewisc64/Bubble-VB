Public Class Cell

    Private b As Bubble

    Property Bubble As Bubble
        Get
            Return b
        End Get
        Set(value As Bubble)
            b = value
            setBubble()
        End Set
    End Property

    Public ix As Integer
    Public iy As Integer

    Property x As Integer
        Get
            Return b.x
        End Get
        Set(value As Integer)
            b.x = value
        End Set
    End Property

    Property y As Integer
        Get
            Return b.y
        End Get
        Set(value As Integer)
            b.y = value
        End Set
    End Property

    Public Sub setBubble()
        If Not IsNothing(b) Then
            b.moving = False
            b.x = ix * b.radius * 2
            b.x += ((iy + 2) Mod 2) * b.radius
            b.y = iy * b.radius * 2
            b.y -= iy * (1 / 3) * b.radius * Math.Cos(Math.PI / 6)
        End If
    End Sub

    Public Sub New(bubble As Bubble, ix As Integer, iy As Integer)
        Me.ix = ix
        Me.iy = iy
        Me.Bubble = bubble
    End Sub

End Class
