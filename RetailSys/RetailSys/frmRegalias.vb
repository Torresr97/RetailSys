Imports System.Data.SqlClient
Imports System.Text
Imports System.Security.Cryptography
Public Class frmRegalias
    Dim ds As New DataSet
    Dim sqlclientes = "SELECT idcliente, cliente FROM clientes"
    Dim daclientes As New SqlDataAdapter

    Public Sub New()
        InitializeComponent()
        daclientes = New SqlDataAdapter(sqlclientes, My.Settings.cnxDB)
    End Sub
    Private Sub frmRegalias_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        daclientes.Fill(ds, "clientes")
        cboCliente.DataSource = ds.Tables("clientes")
        cboCliente.DisplayMember = "cliente"
        cboCliente.ValueMember = "idcliente"

    End Sub

    Private Sub txtIdarticulo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtIdarticulo.KeyPress
        If e.KeyChar = Chr(13) Then
            llenarArticulo()

        End If
    End Sub
    Private Sub txtIdarticulo_Leave(sender As Object, e As EventArgs) Handles txtIdarticulo.Leave
        llenarArticulo()
    End Sub

    Private Sub txtCantidad_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtCantidad.KeyPress
        If e.KeyChar = Chr(13) Then
            txtCantidad.Text = Val(txtCantidad.Text)
            BtnAgregar.PerformClick()
        End If
    End Sub




    Private Sub BtnAgregar_Click(sender As Object, e As EventArgs) Handles BtnAgregar.Click

        If txtIdarticulo.Text <> "" And txtArticulo.Text <> "" And Val(txtCantidad.Text) <> 0 Then
            Dim continuar = True
            For Each arti As DataGridViewRow In grdDetalle.Rows
                If arti.Cells(0).Value.ToString = txtIdarticulo.Text Then
                    continuar = False

                End If
            Next

            If continuar Then
                Dim idarticulo = txtIdarticulo.Text
                Dim articulo = txtArticulo.Text
                Dim cantidad = Val(txtCantidad.Text)



                grdDetalle.Rows.Add(New Object() {idarticulo, articulo, cantidad})
                txtArticulo.Clear()
                txtCantidad.Clear()
                txtIdarticulo.Clear()
                txtIdarticulo.Focus()
            Else
                MsgBox("El articulo ya esta agregado")
                txtIdarticulo.Focus()
                txtIdarticulo.SelectAll()
            End If




        End If
    End Sub



    Private Sub btnAplicar_Click(sender As Object, e As EventArgs) Handles btnAplicar.Click
        If MsgBox("Desea aplicar la transaccion", MsgBoxStyle.OkCancel, "Continuar") = MsgBoxResult.Ok Then
            If txtNumeroDocto.Text <> "" Then

                Dim sqlEncabezado = "INSERT INTO regalias (idcliente,idusuario,nodocumento,fecha) " &
                             "VALUES (@idcliente,'admin',@nodocumento,@fecha) "


                Dim sqlDetalle = "INSERT INTO detalleregalias (idregalias,idarticulo,articulo,cantidad ,precio) " &
                     "VALUES (@idregalias,@idarticulo,@articulo,@cantidad,'0')"


                Dim sqlExistencias = "UPDATE articulos SET cantidad = cantidad - @cantidad WHERE idarticulo = @idarticulo"

                Dim sqlkardex = "INSERT INTO kardex (idarticulo,entrasale,tipotrasaccion,fecha,cantidad)" &
                  "VALUES (@idarticulo,'0','REGALIA',@fecha,@cantidad)"

                Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)



                    Try

                        'Declaramos el objeto que realiza el query con la conexion abierta.
                        Dim cmdEncabezado As New SqlClient.SqlCommand(sqlEncabezado, con)
                        Dim cmdDetalle As New SqlClient.SqlCommand(sqlDetalle, con)
                        Dim cmdExistencias As New SqlClient.SqlCommand(sqlExistencias, con)
                        Dim cmdkardex As New SqlClient.SqlCommand(sqlkardex, con)

                        con.Open()
                        Dim cmdId As New SqlClient.SqlCommand("SELECT @@IDENTITY", con)


                        cmdEncabezado.Parameters.AddWithValue("@idcliente", Val(cboCliente.SelectedValue))
                        cmdEncabezado.Parameters.AddWithValue("@nodocumento", txtNumeroDocto.Text)
                        cmdEncabezado.Parameters.AddWithValue("@fecha", dtpFechaVenta.Value)
                        cmdEncabezado.ExecuteNonQuery()




                        Dim idregalia As Integer = cmdId.ExecuteScalar()


                        'recorremos el detalle y lo insertamos
                        For Each arti As DataGridViewRow In grdDetalle.Rows
                            Try
                                cmdDetalle.Parameters.Clear()
                                cmdExistencias.Parameters.Clear()
                            Catch ex As Exception
                            End Try

                            cmdDetalle.Parameters.AddWithValue("@idregalias", idregalia)
                            cmdDetalle.Parameters.AddWithValue("@idarticulo", arti.Cells(0).Value.ToString)
                            cmdDetalle.Parameters.AddWithValue("@articulo", arti.Cells(1).Value.ToString)
                            cmdDetalle.Parameters.AddWithValue("@cantidad", arti.Cells(2).Value)

                            cmdDetalle.ExecuteNonQuery()

                            cmdExistencias.Parameters.AddWithValue("@idarticulo", arti.Cells(0).Value.ToString)
                            cmdExistencias.Parameters.AddWithValue("@cantidad", arti.Cells(2).Value.ToString)
                            cmdExistencias.ExecuteNonQuery()

                            cmdkardex.Parameters.AddWithValue("@idarticulo", arti.Cells(0).Value.ToString)
                            cmdkardex.Parameters.AddWithValue("@fecha", dtpFechaVenta.Value)
                            cmdkardex.Parameters.AddWithValue("@cantidad", arti.Cells(2).Value.ToString)

                            cmdkardex.ExecuteNonQuery()



                        Next
                    Catch ex As Exception
                        MsgBox("Error al realizar la consulta: " & ex.Message, MsgBoxStyle.Critical, "Error")
                    Finally
                        con.Close()
                    End Try
                End Using



            End If
        End If
        limpiar()
    End Sub


    Private Sub llenarArticulo()
        Dim strsql = "SELECT articulo FROM articulos WHERE idarticulo = @idarticulo"

        Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
            con.Open()
            Try
                'Declaramos el objeto que realiza el query con la conexion abierta.
                Dim cmd As New SqlClient.SqlCommand(strsql, con)
                cmd.Parameters.AddWithValue("@idarticulo", txtIdarticulo.Text)
                Dim reader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                If reader.HasRows Then
                    reader.Read()
                    txtArticulo.Text = reader.GetValue(0)
                    txtCantidad.Focus()
                Else
                    MsgBox("No existe el Articulo ingresado")
                    txtIdarticulo.Focus()
                    txtIdarticulo.SelectAll()
                End If
            Catch ex As Exception
                MsgBox("Error al realizar la consulta: " & ex.Message, MsgBoxStyle.Critical, "Error")
            End Try
        End Using
    End Sub





    Sub limpiar()
        txtIdarticulo.Clear()
        txtArticulo.Clear()
        txtCantidad.Clear()

    End Sub
End Class