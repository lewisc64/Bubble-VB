Public Class Bubble
    Inherits VBGame.Sprite

    Public moving As Boolean

    Public radius As Integer

    Public Shared images As New List(Of Image)

    Private Shared random As New Random()

    Public Sub New(x As Integer, y As Integer, angle As Integer, radius As Integer)
        Me.radius = radius
        Me.x = x
        Me.y = y
        Me.angle = angle
        width = radius * 2
        height = radius * 2
        moving = True
        color = New VBGame.Colors.HSV(0, 100, 100).toRGB()
    End Sub

    Public Overrides Sub draw(Display As VBGame.Display)
        Display.drawCircle(New VBGame.Circle(getCenter(), radius), color)
    End Sub

End Class