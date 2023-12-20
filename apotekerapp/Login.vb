Imports System.Data.SqlClient
Public Class Login
    Dim connectionString As String = "Data Source=MSI; initial catalog=DB_FOOD;Integrated Security=true"
    Private Sub LogActivity(aktifitas As String)
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_Log (aktivitas,waktu,id_User) values (@aktifitas,@waktu,@id)", Conn)
                cmd.Parameters.AddWithValue("@aktifitas", aktifitas)
                cmd.Parameters.AddWithValue("@waktu", DateTime.Now())
                cmd.Parameters.AddWithValue("@id", Module1.User)
                cmd.ExecuteNonQuery()
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub btnLog_Click(sender As Object, e As EventArgs) Handles btnLog.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            If TextBox1.Text = "" Or TextBox2.Text = "" Then
                MsgBox("masukan field dengan lengkap")
            Else
                Using cmd As New SqlCommand("select * from Tbl_User where username=@name and password=@password", Conn)
                    cmd.Parameters.AddWithValue("@name", TextBox1.Text)
                    cmd.Parameters.AddWithValue("@password", TextBox2.Text)
                    Dim sdr As SqlDataReader = cmd.ExecuteReader
                    sdr.Read()
                    If sdr.HasRows Then
                        Module1.User = Convert.ToInt32(sdr("id_User"))
                        LogActivity("Login")
                        If sdr("tipe_user").ToString = "Admin" Then
                            Dasboard.Visible = True
                            Me.Visible = False
                        ElseIf sdr("tipe_user").ToString = "Kasir" Then
                            Transaksi.Visible = True
                            Me.Visible = False
                        ElseIf sdr("tipe_user").ToString = "Gudang" Then
                            Gudang.Visible = True
                            Me.Visible = False
                        End If
                    Else
                        MsgBox("Password atau Username salah")
                    End If
                End Using
            End If
            Conn.Close()
        End Using
    End Sub

    Private Sub Login_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        TextBox1.Text = ""
        TextBox2.Text = ""
    End Sub

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox2.PasswordChar = "•"
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        TextBox1.Text = ""
        TextBox2.Text = ""
    End Sub
End Class
