Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.XtraScheduler
Imports System.Collections
Imports DevExpress.XtraScheduler.Drawing
Imports System.Windows.Forms
Imports System.Drawing
Imports DevExpress.Services

Namespace WindowsFormsApplication1
	Public Class DragDropResourcesHelper
		Private CurrentScheduler As SchedulerControl
		Private CustomFieldName As String

		Public Sub New(ByVal scheduler As SchedulerControl, ByVal fieldName As String)
			CurrentScheduler = scheduler
			CustomFieldName = fieldName

			AddHandler CurrentScheduler.Storage.ResourceCollectionLoaded, AddressOf Storage_ResourceCollectionLoaded
			AddHandler CurrentScheduler.DragOver, AddressOf CurrentScheduler_DragOver
			AddHandler CurrentScheduler.DragDrop, AddressOf CurrentScheduler_DragDrop

			Dim oldMouseHandler As IMouseHandlerService = CType(CurrentScheduler.GetService(GetType(IMouseHandlerService)), IMouseHandlerService)
			If oldMouseHandler IsNot Nothing Then
				Dim newMouseHandler As New DragrDropMouseHandlerService(CurrentScheduler, oldMouseHandler)
				CurrentScheduler.RemoveService(GetType(IMouseHandlerService))
				CurrentScheduler.AddService(GetType(IMouseHandlerService), newMouseHandler)
			End If
		End Sub

		Private Sub CurrentScheduler_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs)
			If e.Data.GetDataPresent(GetType(DevExpress.XtraScheduler.Resource)) Then

				Dim dropHitInfo As SchedulerHitInfo = CurrentScheduler.ActiveView.ViewInfo.CalcHitInfo(CurrentScheduler.PointToClient(New Point(e.X, e.Y)), False)
				If dropHitInfo.HitTest = SchedulerHitTest.ResourceHeader Then
					Dim sourceResource As Resource = TryCast(e.Data.GetData(GetType(DevExpress.XtraScheduler.Resource)), DevExpress.XtraScheduler.Resource)
					Dim targetResource As Resource = dropHitInfo.ViewInfo.Resource
					If sourceResource IsNot targetResource Then
						Dim sourceResourceSortOrder As Object = sourceResource.CustomFields(CustomFieldName)
						sourceResource.CustomFields(CustomFieldName) = targetResource.CustomFields(CustomFieldName)
						targetResource.CustomFields(CustomFieldName) = sourceResourceSortOrder
						ApplySorting()
					End If
				End If
			End If
		End Sub

		Private Sub CurrentScheduler_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs)
			If e.Data.GetDataPresent(GetType(DevExpress.XtraScheduler.Resource)) Then
                e.Effect = DragDropEffects.Move
			End If
		End Sub

		Private Sub ApplySorting()
			CurrentScheduler.Storage.Resources.Items.Sort(New ResourceCustomFieldComparer("SortOrder"))
			CurrentScheduler.ActiveView.LayoutChanged()
		End Sub


		Private Sub Storage_ResourceCollectionLoaded(ByVal sender As Object, ByVal e As EventArgs)
			ApplySorting()
		End Sub
	End Class

	' MOUSE HANDLER SERVICE
	Public Class DragrDropMouseHandlerService
		Inherits MouseHandlerServiceWrapper
		Private provider As IServiceProvider
		Private downHitInfo As SchedulerHitInfo
		Private downHitPoint As Point


		Public Sub New(ByVal provider As IServiceProvider, ByVal service As IMouseHandlerService)
			MyBase.New(service)
			Me.provider = provider
		End Sub

		Public Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
			downHitInfo = Nothing
			downHitPoint = Point.Empty
			Dim hitInfo As SchedulerHitInfo = (TryCast(provider, SchedulerControl)).ActiveView.ViewInfo.CalcHitInfo(e.Location, False)
			If hitInfo.HitTest = SchedulerHitTest.ResourceHeader Then
				downHitInfo = hitInfo
				downHitPoint = e.Location
			Else
				MyBase.OnMouseDown(e)
			End If
		End Sub

		Public Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
			If e.Button = MouseButtons.Left AndAlso downHitInfo IsNot Nothing Then
				Dim dragSize As Size = SystemInformation.DragSize
				Dim dragRect As New Rectangle(New Point(downHitPoint.X - dragSize.Width / 2, downHitPoint.Y - dragSize.Height / 2), dragSize)

				If (Not dragRect.Contains(New Point(e.X, e.Y))) Then
					TryCast(provider, SchedulerControl).DoDragDrop(downHitInfo.ViewInfo.Resource, DragDropEffects.All)
					downHitInfo = Nothing
					downHitPoint = Point.Empty
				End If
			Else
				MyBase.OnMouseMove(e)
			End If
		End Sub
	End Class



	' RESOURECE COMPARER
	Public MustInherit Class ResourceBaseComparer
		Implements IComparer(Of Resource), IComparer
		#Region "IComparer Members"
		Private Function IComparer_Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
			Return CompareCore(x, y)
		End Function
		Public Function Compare(ByVal x As Resource, ByVal y As Resource) As Integer Implements IComparer(Of Resource).Compare
			Return CompareCore(x, y)
		End Function
		#End Region

		Protected Overridable Function CompareCore(ByVal x As Object, ByVal y As Object) As Integer
			Dim xRes As Resource = CType(x, Resource)
			Dim yRes As Resource = CType(y, Resource)

			If xRes Is Nothing OrElse yRes Is Nothing Then
				Return 0
			End If
			If Resource.Empty.Equals(xRes) OrElse Resource.Empty.Equals(yRes) Then
				Return 0
			End If

			Return CompareResources(xRes, yRes)
		End Function

		Protected MustOverride Function CompareResources(ByVal xRes As Resource, ByVal yRes As Resource) As Integer
	End Class

	Public Class ResourceCustomFieldComparer
		Inherits ResourceBaseComparer
		Private customFieldValue As String = ""
		Public Sub New(ByVal parCustomField As String)
			customFieldValue = parCustomField
		End Sub

		Protected Overrides Function CompareResources(ByVal xRes As Resource, ByVal yRes As Resource) As Integer
            Return Convert.ToInt32(xRes.CustomFields(customFieldValue)).CompareTo(Convert.ToInt32(yRes.CustomFields(customFieldValue)))
		End Function
	End Class
End Namespace
