Public Class Bubble
    Inherits VBGame.Sprite

    Public moving As Boolean

    Public radius As Integer

    Public realRadius As Integer

    Public scaledImage As Image

    Public Shared images As New List(Of Image)

    Private Shared random As New Random()

    Public Sub New(x As Integer, y As Integer, angle As Integer, radius As Integer)
        Me.radius = radius
        realRadius = radius * (4 / 5)
        Me.x = x
        Me.y = y
        Me.angle = angle
        width = radius * 2
        height = radius * 2
        moving = True
        Dim c As Integer = random.Next(0, images.Count)
        image = images(c)
        color = color.FromArgb(c)
        speed = 10
        scaledImage = VBGame.Images.resizeImage(image, 4 / 5, False)
    End Sub

    Public Shadows Sub draw(Display As VBGame.Display)
        'Display.drawCircle(New VBGame.Circle(getCenter(), realRadius), VBGame.Colors.black)
        Display.blitCentered(scaledImage, getCenter())
        'If radius = 15 Then
        '    Display.blit(image, getXY())
        'Else
        '    Display.blit(image, getRect())
        'End If
    End Sub

    Public Function handle(ByRef grid As Grid)
        move(True)
        If keepInBounds(New Rectangle(New Point(0, 0), grid.bounds), True, True) Then
            VBGame.Assets.sounds("bounce").play(False, True)
        End If
        For Each Cell As Cell In grid.exposed
            If VBGame.Collisions.rect(getRect(), Cell.Bubble.getRect()) Then
                If VBGame.Collisions.circle(New VBGame.Circle(getCenter(), realRadius), New VBGame.Circle(Cell.Bubble.getCenter(), Cell.Bubble.realRadius)) Then
                    grid.snapBubble(Me)
                    VBGame.Assets.sounds("snap").play(False, True)
                    Return True
                End If
            End If
        Next
        Return False
    End Function

End Class