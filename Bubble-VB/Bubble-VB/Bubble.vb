Public Class Bubble
    Inherits VBGame.Sprite

    Public moving As Boolean

    Public radius As Integer

    Public realRadius As Integer

    Public scaledImage As Image

    Public Shared images As New List(Of Image)

    Private Shared random As New Random()

    Public popped As Boolean

    Public Shadows color As Integer

    Public scale As Single = 4 / 5
    Private oscale As Single = 4 / 5

    Public Sub New(x As Integer, y As Integer, angle As Integer, radius As Integer, availableColors As List(Of Integer))
        Me.radius = radius
        realRadius = radius * scale
        Me.x = x
        Me.y = y
        Me.angle = angle
        width = radius * 2
        height = radius * 2
        moving = True

        color = availableColors(random.Next(0, availableColors.Count))
        image = images(color)
        speed = 10

        scaledImage = VBGame.Images.resizeImage(image, scale, False)
    End Sub

    Public Overridable Sub onFire()
    End Sub

    Public Overridable Shadows Sub draw(Display As VBGame.Display)
        'Display.drawCircle(New VBGame.Circle(getCenter(), realRadius), VBGame.Colors.black)
        If scale = oscale Then
            Display.blitCentered(scaledImage, getCenter())
        Else
            Display.blitCentered(VBGame.Images.resizeImage(scaledImage, scale, False), getCenter())
        End If

    End Sub

    Public Overridable Function handle(ByRef grid As Grid, player As Player)
        move(True)
        If keepInBounds(New Rectangle(New Point(0, 0), grid.bounds), True, True) Then
            VBGame.Assets.sounds("bounce").play(False, True)
            If y = 0 Then
                grid.snapBubble(Me, player)
                VBGame.Assets.sounds("snap").play(False, True)
                Return True
            End If
        End If
        For Each Cell As Cell In grid.exposed
            If VBGame.Collisions.rect(getRect(), Cell.Bubble.getRect()) Then
                If VBGame.Collisions.circle(New VBGame.Circle(getCenter(), realRadius), New VBGame.Circle(Cell.Bubble.getCenter(), Cell.Bubble.realRadius)) Then
                    grid.snapBubble(Me, player)
                    VBGame.Assets.sounds("snap").play(False, True)
                    Return True
                End If
            End If
        Next
        Return False
    End Function

End Class