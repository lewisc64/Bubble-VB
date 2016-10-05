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
        Dim c As Integer = random.Next(0, images.Count)
        image = images(c)
        color = color.FromArgb(c)

    End Sub

    Public Overrides Sub draw(Display As VBGame.Display)
        'Display.drawCircle(New VBGame.Circle(getCenter(), radius), color)
        Display.blit(image, getRect())
    End Sub

End Class