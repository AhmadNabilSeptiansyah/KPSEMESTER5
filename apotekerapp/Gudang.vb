Imports System.Data.SqlClient
Public Class Gudang
    Dim connectionString As String = "Data Source=MSI;initial catalog=DB_FOOD;Integrated Security=true"

    Sub kondisiAwal()
        Using Conn As New SqlConnection(connectionString)
            Using sda As New SqlDataAdapter("select * from Tbl_barang order by waktu desc", Conn)
                Using ds As New DataSet()
                    ds.Clear()
                    sda.Fill(ds, "Tbl_barang")
                    DataGridView1.DataSource = (ds.Tables("Tbl_barang"))
                End Using
            End Using
            cmbSatuan.Text = ""
            cmbSatuan.Items.Clear()
            cmbSatuan.Items.Add("Botol")
            cmbSatuan.Items.Add("Pcs")

            cmbSatuan.Enabled = False
            txtNama.Enabled = False
            txtJumlah.Enabled = False
            txtharga.Enabled = False
            txtKb.Enabled = False
            DateTimePicker1.Enabled = False
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
    Sub kosongkanData()
        cmbSatuan.Text = ""
        txtNama.Text = ""
        txtJumlah.Text = ""
        txtharga.Text = ""
        txtKb.Text = ""
        DateTimePicker1.Text = ""
    End Sub
    Sub btnEnabled()
        cmbSatuan.Enabled = True
        txtNama.Enabled = True
        txtJumlah.Enabled = True
        txtharga.Enabled = True
        DateTimePicker1.Enabled = True
    End Sub
    Private Sub Gudang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kondisiAwal()
    End Sub
    Private Function cekKd() As String
        'Dim str As String = ""
        'Dim rnd As New Random
        'For i As Integer = 1 To 6
        'Str &= rnd.Next(0, 9).ToString()
        'Next
        'Return str
        Dim random As New Random()
        Dim randomNumber As Integer = random.Next(1, 99999)
        Return randomNumber
    End Function
    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            If btnTambah.Text = "Tambah" Then
                btnTambah.Text = "Simpan"
                btnEdit.Enabled = False
                btnHapus.Enabled = False
                kosongkanData()
                Dim kodeUnik As String = cekKd()
                txtKb.Text = "BRG" & kodeUnik
                btnEnabled()
            Else
                If txtNama.Text = "" Or txtharga.Text = "" Or cmbSatuan.Text = "" Or txtJumlah.Text = "" Or txtKb.Text = "" Then
                    MsgBox("Text Field Masih Kosong")
                Else
                    If MessageBox.Show("Yakin tambah data ?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        Using cmd As New SqlCommand("insert into Tbl_barang (kode_barang,nama_barang,expired_date,jumlah_barang,satuan,harga_satuan,waktu) values(@kd,@nama,@exp,@jumlah,@satuan,@harga,@waktu)", Conn)
                            cmd.Parameters.AddWithValue("@kd", txtKb.Text)
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
                            cmd.Parameters.AddWithValue("@exp", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                            cmd.Parameters.AddWithValue("@jumlah", txtJumlah.Text)
                            cmd.Parameters.AddWithValue("@satuan", cmbSatuan.Text)
                            cmd.Parameters.AddWithValue("@harga", txtharga.Text)
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
                btnEnabled()
            Else
                If txtNama.Text = "" Or txtharga.Text = "" Or cmbSatuan.Text = "" Or txtJumlah.Text = "" Or txtKb.Text = "" Then
                    MsgBox("Data belum lengkap!")
                Else
                    If MessageBox.Show("Apakah Anda ingin menyimpan Perubahan?", "info", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                        Using cmd As New SqlCommand("update Tbl_barang Set kode_barang=@kd,nama_barang=@nama,expired_date=@exp,jumlah_barang=@jumlah,satuan=@satuan,harga_satuan=@harga,waktu=@waktu where id_barang=@id", Conn)
                            cmd.Parameters.AddWithValue("@kd", txtKb.Text)
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
                            cmd.Parameters.AddWithValue("@exp", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                            cmd.Parameters.AddWithValue("@jumlah", txtJumlah.Text)
                            cmd.Parameters.AddWithValue("@satuan", cmbSatuan.Text)
                            cmd.Parameters.AddWithValue("@harga", txtharga.Text)
                            cmd.Parameters.AddWithValue("@waktu", Date.Now)
                            cmd.Parameters.AddWithValue("@id", txtId.Text)
                            cmd.ExecuteNonQuery()
                            kosongkanData()
                            btnEnabled()
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
                Using cmd As New SqlCommand("delete from Tbl_barang where id_barang = @id", Conn)
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
        txtKb.Text = IIf(IsDBNull(DataGridView1.Item(1, i).Value), "", DataGridView1.Item(1, i).Value)
        txtNama.Text = IIf(IsDBNull(DataGridView1.Item(2, i).Value), "", DataGridView1.Item(2, i).Value)
        DateTimePicker1.Text = IIf(IsDBNull(DataGridView1.Item(3, i).Value), "", DataGridView1.Item(3, i).Value)
        txtJumlah.Text = IIf(IsDBNull(DataGridView1.Item(4, i).Value), "", DataGridView1.Item(4, i).Value)

        cmbSatuan.Text = IIf(IsDBNull(DataGridView1.Item(5, i).Value), "", DataGridView1.Item(5, i).Value)
        txtharga.Text = IIf(IsDBNull(DataGridView1.Item(6, i).Value), "", DataGridView1.Item(6, i).Value)
    End Sub

    Private Sub txtCari_TextChanged(sender As Object, e As EventArgs) Handles txtCari.TextChanged

        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("Select * from Tbl_barang where nama_barang Like Concat('%' ,@search,'%')", Conn)
                cmd.Parameters.AddWithValue("@search", txtCari.Text)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                sdr.Read()
                If sdr.HasRows Then
                    txtId.Text = sdr("id_barang")
                    txtKb.Text = sdr("kode_barang")
                    txtNama.Text = sdr("nama_barang")
                    DateTimePicker1.Text = sdr("expired_date")
                    txtJumlah.Text = sdr("jumlah_barang")
                    cmbSatuan.Text = sdr("satuan")
                    txtharga.Text = sdr("harga_satuan")
                End If
                sdr.Close()
            End Using
            Using cmd As New SqlCommand("Select * from Tbl_barang where nama_barang Like Concat('%' ,@search,'%')", Conn)
                cmd.Parameters.AddWithValue("@search", txtCari.Text)
                Dim sda As New SqlDataAdapter(cmd)
                Dim table As New DataTable
                sda.Fill(table)
                DataGridView1.DataSource = table
            End Using
            Conn.Close()
        End Using
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        LogActivity("Logout")
        Me.Visible = False
        Login.Visible = True
    End Sub

    Private Sub txtharga_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtharga.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Enter) Then
            txtharga.Text = FormatCurrency(txtharga.Text)
        End If
    End Sub

    Private Sub Gudang_Move(sender As Object, e As EventArgs) Handles Me.Move
        kondisiAwal()
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