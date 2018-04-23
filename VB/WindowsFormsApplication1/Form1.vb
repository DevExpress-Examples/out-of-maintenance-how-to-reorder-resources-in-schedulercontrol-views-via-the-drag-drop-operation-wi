Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler

Namespace WindowsFormsApplication1
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
			Dim ddHelper As New DragDropResourcesHelper(schedulerControl1, "SortOrder")
		End Sub

		Public Shared RandomInstance As New Random()

		Private CustomResourceCollection As New BindingList(Of CustomResource)()
		Private CustomEventList As New BindingList(Of CustomAppointment)()

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			InitResources()
			InitAppointments()
			schedulerControl1.Start = DateTime.Now
			schedulerControl1.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource

			schedulerControl1.ActiveViewType = SchedulerViewType.Timeline
		End Sub

		Private Sub InitResources()
			Dim mappings As ResourceMappingInfo = Me.schedulerStorage1.Resources.Mappings
			mappings.Id = "ResID"
			mappings.Caption = "Name"

			Me.schedulerStorage1.Resources.CustomFieldMappings.Add(New ResourceCustomFieldMapping("SortOrder", "ResSortOrder", FieldValueType.String))

			CustomResourceCollection.Add(CreateCustomResource(1, "Max Fowler", Color.PowderBlue, 3))
			CustomResourceCollection.Add(CreateCustomResource(2, "Nancy Drewmore", Color.PaleVioletRed, 1))
			CustomResourceCollection.Add(CreateCustomResource(3, "Pak Jang", Color.PeachPuff, 2))
			Me.schedulerStorage1.Resources.DataSource = CustomResourceCollection
		End Sub

		Private Function CreateCustomResource(ByVal res_id As Integer, ByVal caption As String, ByVal ResColor As Color, ByVal sortOrder As Integer) As CustomResource
			Dim cr As New CustomResource()
			cr.ResID = res_id
			cr.Name = caption
			cr.ResSortOrder = sortOrder
			Return cr
		End Function



		Private Sub InitAppointments()
			Dim mappings As AppointmentMappingInfo = Me.schedulerStorage1.Appointments.Mappings
			mappings.Start = "StartTime"
			mappings.End = "EndTime"
			mappings.Subject = "Subject"
			mappings.AllDay = "AllDay"
			mappings.Description = "Description"
			mappings.Label = "Label"
			mappings.Location = "Location"
			mappings.RecurrenceInfo = "RecurrenceInfo"
			mappings.ReminderInfo = "ReminderInfo"
			mappings.ResourceId = "OwnerId"
			mappings.Status = "Status"
			mappings.Type = "EventType"

			GenerateEvents(CustomEventList)
			Me.schedulerStorage1.Appointments.DataSource = CustomEventList
		End Sub


		Private Sub GenerateEvents(ByVal eventList As BindingList(Of CustomAppointment))
			Dim count As Integer = schedulerStorage1.Resources.Count

			For i As Integer = 0 To count - 1
				Dim resource As Resource = schedulerStorage1.Resources(i)
				Dim subjPrefix As String = resource.Caption & "'s "
				eventList.Add(CreateEvent(subjPrefix & "meeting", resource.Id, 2, 5))
				eventList.Add(CreateEvent(subjPrefix & "travel", resource.Id, 3, 6))
				eventList.Add(CreateEvent(subjPrefix & "phone call", resource.Id, 0, 10))
			Next i
		End Sub
		Private Function CreateEvent(ByVal subject As String, ByVal resourceId As Object, ByVal status As Integer, ByVal label As Integer) As CustomAppointment
			Dim apt As New CustomAppointment()
			apt.Subject = subject
			apt.OwnerId = resourceId
			Dim rnd As Random = RandomInstance
			Dim rangeInMinutes As Integer = 60 * 24
			apt.StartTime = DateTime.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes))
			apt.EndTime = apt.StartTime + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes \ 4))
			apt.Status = status
			apt.Label = label
			Return apt
		End Function
	End Class
End Namespace
