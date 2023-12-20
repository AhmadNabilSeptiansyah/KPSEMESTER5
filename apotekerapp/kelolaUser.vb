Imports System.Data.SqlClient
Imports System.Reflection

Public Class kelolaUser
    Dim connectionString As String = "Data Source=MSI; initial catalog=DB_FOOD; Integrated Security=true"
    Sub kondisiAwal()
        Using Conn As New SqlConnection(connectionString)
            Using sda As New SqlDataAdapter("select id_User,tipe_user,nama,alamat,telepon,username,password from Tbl_User order by waktu desc", Conn)
                Using ds As New DataSet()
                    ds.Clear()
                    sda.Fill(ds, "Tbl_User")
                    DataGridView1.DataSource = (ds.Tables("Tbl_User"))
                End Using
            End Using
            cmbUser.Text = ""
            cmbUser.Items.Clear()
            cmbUser.Items.Add("Gudang")
            cmbUser.Items.Add("Kasir")

            cmbUser.Enabled = False
            txtNama.Enabled = False
            txtAlamat.Enabled = False
            txtPassword.Enabled = False
            txtUsername.Enabled = False
            txtTelp.Enabled = False
        End Using
    End Sub
    Private Sub LogActivity(aktifitas As String)
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_Log (waktu,aktivitas,id_User) values (@waktu,@aktifitas,@id)", Conn)
                cmd.Parameters.AddWithValue("@waktu", DateTime.Now())
                cmd.Parameters.AddWithValue("@aktifitas", aktifitas)
                cmd.Parameters.AddWithValue("@id", Module1.User)
                cmd.ExecuteNonQuery()
            End Using
            Conn.Close()
        End Using
    End Sub
    Sub btnEnable()
        cmbUser.Enabled = True
        txtNama.Enabled = True
        txtAlamat.Enabled = True
        txtPassword.Enabled = True
        txtUsername.Enabled = True
        txtTelp.Enabled = True
    End Sub
    Sub kosongkanData()
        cmbUser.Text = ""
        txtNama.Text = ""
        txtAlamat.Text = ""
        txtPassword.Text = ""
        txtTelp.Text = ""
        txtUsername.Text = ""
    End Sub

    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            If btnTambah.Text = "Tambah" Then
                btnTambah.Text = "Simpan"
                btnEdit.Enabled = False
                btnHapus.Enabled = False
                kosongkanData()
                btnEnable()
            Else
                If txtNama.Text = "" Or txtTelp.Text = "" Or cmbUser.Text = "" Or txtPassword.Text = "" Or txtUsername.Text = "" Or txtAlamat.Text = "" Then
                    MsgBox("Text Field Masih Kosong")
                Else
                    If MessageBox.Show("Yakin tambah data ?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        Using cmd As New SqlCommand("insert into Tbl_User (tipe_User,nama,alamat,telepon,username,password,waktu) values(@tipe,@nama,@alamat,@telpon,@username,@password,@waktu)", Conn)
                            cmd.Parameters.AddWithValue("@tipe", cmbUser.Text)
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
                            cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text)
                            cmd.Parameters.AddWithValue("@telpon", txtTelp.Text)
                            cmd.Parameters.AddWithValue("@username", txtUsername.Text)
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                            cmd.Parameters.AddWithValue("@waktu", Date.Now)
                            cmd.ExecuteNonQuery()
                            kondisiAwal()
                            btnTambah.Text = "Tambah"
                            btnEdit.Enabled = True
                            btnHapus.Enabled = True
                            kosongkanData()
                        End Using
                    End If
                End If
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            If btnEdit.Text = "Edit" Then
                btnEdit.Text = "Simpan"
                btnHapus.Enabled = False
                btnTambah.Enabled = False
                btnEnable()
            Else
                If cmbUser.Text = "" Or txtNama.Text = "" Or txtPassword.Text = "" Or txtTelp.Text = "" Or txtUsername.Text = "" Or txtAlamat.Text = "" Then
                    MsgBox("Data belum lengkap!")
                Else
                    If MessageBox.Show("Apakah Anda ingin menyimpan Perubahan?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        Using cmd As New SqlCommand("update Tbl_User set tipe_user=@tipe,nama=@nama,alamat=@alamat,telepon=@telpon,username=@Username,password=@password,waktu=@waktu where id_User=@id", Conn)
                            cmd.Parameters.AddWithValue("@id", txtId.Text)
                            cmd.Parameters.AddWithValue("@tipe", cmbUser.Text)
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
                            cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text)
                            cmd.Parameters.AddWithValue("@telpon", txtTelp.Text)
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text)
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                            cmd.Parameters.AddWithValue("@waktu", DateTime.Now())
                            cmd.ExecuteNonQuery()
                            kosongkanData()
                            btnEnable()
                            kondisiAwal()
                            btnEdit.Text = "Edit"
                            btnHapus.Enabled = True
                            btnTambah.Enabled = True
                        End Using
                    End If
                End If
            End If
        End Using
    End Sub

    Private Sub btnHapus_Click(sender As Object, e As EventArgs) Handles btnHapus.Click
        If MessageBox.Show("Apakah anda yakin ingin menghapus data?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            Using Conn As New SqlConnection(connectionString)
                Conn.Open()
                Using cmd As New SqlCommand("delete from Tbl_user where id_user = @id", Conn)
                    cmd.Parameters.AddWithValue("@id", txtId.Text)
                    cmd.ExecuteNonQuery()
                    kondisiAwal()
                    kosongkanData()
                End Using
            End Using
        End If
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        Dim i As Integer
        i = DataGridView1.CurrentRow.Index

        txtId.Text = IIf(IsDBNull(DataGridView1.Item(0, i).Value), "", DataGridView1.Item(0, i).Value)
        cmbUser.Text = IIf(IsDBNull(DataGridView1.Item(1, i).Value), "", DataGridView1.Item(1, i).Value)
        txtNama.Text = IIf(IsDBNull(DataGridView1.Item(2, i).Value), "", DataGridView1.Item(2, i).Value)
        txtAlamat.Text = IIf(IsDBNull(DataGridView1.Item(3, i).Value), "", DataGridView1.Item(3, i).Value)
        txtTelp.Text = IIf(IsDBNull(DataGridView1.Item(4, i).Value), "", DataGridView1.Item(4, i).Value)

        txtUsername.Text = IIf(IsDBNull(DataGridView1.Item(5, i).Value), "", DataGridView1.Item(5, i).Value)
        txtPassword.Text = IIf(IsDBNull(DataGridView1.Item(6, i).Value), "", DataGridView1.Item(6, i).Value)
    End Sub

    Private Sub txtCari_TextChanged(sender As Object, e As EventArgs) Handles txtCari.TextChanged
        Dim ds As New DataSet
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_User where nama Like Concat('%' ,@search,'%')", Conn)
                cmd.Parameters.AddWithValue("@search", txtCari.Text)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                sdr.Read()
                If sdr.HasRows Then
                    txtId.Text = sdr("id_user")
                    cmbUser.Text = sdr("tipe_user")
                    txtNama.Text = sdr("nama")
                    txtAlamat.Text = sdr("alamat")
                    txtUsername.Text = sdr("username")
                    txtTelp.Text = sdr("telepon")
                    txtPassword.Text = sdr("password")
                End If
                sdr.Close()
            End Using
            Using cmd As New SqlCommand("select * from Tbl_User where nama Like Concat('%' ,@search,'%')", Conn)
                cmd.Parameters.AddWithValue("@search", txtCari.Text)
                Dim sda As New SqlDataAdapter(cmd)
                Dim table As New DataTable
                sda.Fill(table)
                DataGridView1.DataSource = table
            End Using
            Conn.Close()
        End Using
    End Sub

    Private Sub btnKelolaLaporan_Click(sender As Object, e As EventArgs) Handles btnKelolaLaporan.Click
        Me.Visible = False
        kelolaLaporan.Visible = True
    End Sub
    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        LogActivity("Logout")
        Me.Visible = False
        Login.Visible = True
    End Sub

    Private Sub kelolaUser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kondisiAwal()
        btnKelolaUser.BackColor = Color.LightYellow
    End Sub

    Private Sub btnLog_Click(sender As Object, e As EventArgs) Handles btnLog.Click
        Me.Visible = False
        Dasboard.Visible = True
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If btnEdit.Text = "Simpan" Or btnTambah.Text = "Simpan" Then
            kosongkanData()
            kondisiAwal()
            btnEdit.Text = "Edit"
            btnTambah.Text = "Tambah"
        End If
    End Sub
End Class