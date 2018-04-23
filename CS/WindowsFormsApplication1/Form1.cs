using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            DragDropResourcesHelper ddHelper = new DragDropResourcesHelper(schedulerControl1, "SortOrder");
        }

        public static Random RandomInstance = new Random();

        private BindingList<CustomResource> CustomResourceCollection = new BindingList<CustomResource>();
        private BindingList<CustomAppointment> CustomEventList = new BindingList<CustomAppointment>();

        private void Form1_Load(object sender, EventArgs e) {
            InitResources();
            InitAppointments();
            schedulerControl1.Start = DateTime.Now;
            schedulerControl1.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource;

            schedulerControl1.ActiveViewType = SchedulerViewType.Timeline;
        }

        private void InitResources() {
            ResourceMappingInfo mappings = this.schedulerStorage1.Resources.Mappings;
            mappings.Id = "ResID";
            mappings.Caption = "Name";

            this.schedulerStorage1.Resources.CustomFieldMappings.Add(new ResourceCustomFieldMapping("SortOrder", "ResSortOrder", FieldValueType.String));

            CustomResourceCollection.Add(CreateCustomResource(1, "Max Fowler", Color.PowderBlue, 3));
            CustomResourceCollection.Add(CreateCustomResource(2, "Nancy Drewmore", Color.PaleVioletRed, 1));
            CustomResourceCollection.Add(CreateCustomResource(3, "Pak Jang", Color.PeachPuff, 2));
            this.schedulerStorage1.Resources.DataSource = CustomResourceCollection;
        }

        private CustomResource CreateCustomResource(int res_id, string caption, Color ResColor, int sortOrder) {
            CustomResource cr = new CustomResource();
            cr.ResID = res_id;
            cr.Name = caption;
            cr.ResSortOrder = sortOrder;
            return cr;
        }



        private void InitAppointments() {
            AppointmentMappingInfo mappings = this.schedulerStorage1.Appointments.Mappings;
            mappings.Start = "StartTime";
            mappings.End = "EndTime";
            mappings.Subject = "Subject";
            mappings.AllDay = "AllDay";
            mappings.Description = "Description";
            mappings.Label = "Label";
            mappings.Location = "Location";
            mappings.RecurrenceInfo = "RecurrenceInfo";
            mappings.ReminderInfo = "ReminderInfo";
            mappings.ResourceId = "OwnerId";
            mappings.Status = "Status";
            mappings.Type = "EventType";

            GenerateEvents(CustomEventList);
            this.schedulerStorage1.Appointments.DataSource = CustomEventList;
        }


        private void GenerateEvents(BindingList<CustomAppointment> eventList) {
            int count = schedulerStorage1.Resources.Count;

            for(int i = 0; i < count; i++) {
                Resource resource = schedulerStorage1.Resources[i];
                string subjPrefix = resource.Caption + "'s ";                
                eventList.Add(CreateEvent(subjPrefix + "meeting", resource.Id, 2, 5));
                eventList.Add(CreateEvent(subjPrefix + "travel", resource.Id, 3, 6));
                eventList.Add(CreateEvent(subjPrefix + "phone call", resource.Id, 0, 10));
            }
        }
        private CustomAppointment CreateEvent(string subject, object resourceId, int status, int label) {
            CustomAppointment apt = new CustomAppointment();
            apt.Subject = subject;
            apt.OwnerId = resourceId;
            Random rnd = RandomInstance;
            int rangeInMinutes = 60 * 24;
            apt.StartTime = DateTime.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes));
            apt.EndTime = apt.StartTime + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes / 4));
            apt.Status = status;
            apt.Label = label;
            return apt;
        }
    }
}
