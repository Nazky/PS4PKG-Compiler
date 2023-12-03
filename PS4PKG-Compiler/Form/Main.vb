Imports System.IO
Imports System.IO.Compression
Imports System.Threading

Public Class Main
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If Directory.Exists("tools") Then
            Else
                File.WriteAllBytes("tools.zip", My.Resources.tools)
                ZipFile.ExtractToDirectory("tools.zip", "tools")
                File.Delete("tools.zip")
            End If
            If TextBox1.Text = "" Or TextBox1.Text = "Drag and drop" Then
                MsgBox("Please put a valid path.", MsgBoxStyle.Critical)
            ElseIf Directory.Exists(TextBox1.Text) Then
                Dim spkg As New SaveFileDialog
                spkg.Title = "Choose where you want to save you'r package."
                spkg.FileName = Path.GetFileNameWithoutExtension(TextBox1.Text)
                spkg.Filter = "PS4/PS5 Package (*.pkg)|*.pkg"
                spkg.AddExtension = True

                If spkg.ShowDialog = DialogResult.OK Then
                    cpkg(spkg.FileName, TextBox1.Text)
                    'MsgBox(spkg.FileName)
                End If
            Else
                MsgBox("Folder not found.", MsgBoxStyle.Critical)
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Sub cpkg(pkgn As String, ppkg As String)
        Try
            gen(ppkg)
            cid(ppkg)
            syscmd("tools\orbis-pub-cmd.exe", "img_create " & Chr(34) & ppkg & ".gp4" & Chr(34) & " " & Chr(34) & pkgn & Chr(34), Me)
            File.Delete(ppkg & ".gp4")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    Sub gen(ppkg As String)
        Try
            syscmd("tools\gengp4.exe", """" & ppkg & """", Me)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    Sub cid(ppkg As String)
        Try
            Dim gp4 As String = System.IO.File.ReadAllText(ppkg & ".gp4")
            System.IO.File.WriteAllText(ppkg & ".gp4", gp4.Replace("<scenarios default_id=""1"">", "<scenarios default_id=""0"">"))
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    Shared Sub syscmd(fexec As String, farg As String, frm As Main)
        Dim oProcess As New Process()
        Dim oStartInfo As New ProcessStartInfo(fexec, farg)
        oStartInfo.UseShellExecute = False
        oStartInfo.RedirectStandardOutput = True
        oStartInfo.WindowStyle = ProcessWindowStyle.Hidden
        oStartInfo.CreateNoWindow = True
        oStartInfo.RedirectStandardError = True
        oProcess.StartInfo = oStartInfo
        oProcess.Start()
        oProcess.WaitForExit()

        Using oStreamReader As System.IO.StreamReader = oProcess.StandardOutput
            frm.Invoke(Sub() frm.RichTextBox1.Text = frm.RichTextBox1.Text & oStreamReader.ReadToEnd())
        End Using
        'Console.WriteLine(sOutput)
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged
        RichTextBox1.SelectionStart = RichTextBox1.Text.Length
        RichTextBox1.ScrollToCaret()
    End Sub

    Private Sub TextBox1_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox1_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            TextBox1.Text = path
        Next
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://twitter.com/NazkyYT")
    End Sub
End Class
