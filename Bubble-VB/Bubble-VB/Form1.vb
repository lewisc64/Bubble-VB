Imports System.Threading

Public Class Form1

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg <> &HF Then
            MyBase.WndProc(m)
        End If
    End Sub

    Public display As VBGame.Display
    Public thread As New Thread(AddressOf gameloop)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        display = New VBGame.Display(Me, New Size(800, 600), "Bubble-VB")
        thread.Start()
    End Sub

    Public Sub gameloop()

        Dim grid As Grid = New Grid(display.width / 20 - 1, display.height / 20 - 1, 10, 20)

        While True

            display.fill(VBGame.Colors.black)

            grid.draw(display)

            display.update()
            display.clockTick(60)

        End While

    End Sub

End Class
