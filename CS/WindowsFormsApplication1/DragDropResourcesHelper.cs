using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraScheduler;
using System.Collections;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Services;

namespace WindowsFormsApplication1 {
    public class DragDropResourcesHelper {
        private SchedulerControl CurrentScheduler;
        private string CustomFieldName;

        public DragDropResourcesHelper(SchedulerControl scheduler, string fieldName) {
            CurrentScheduler = scheduler;
            CustomFieldName = fieldName;

            CurrentScheduler.Storage.ResourceCollectionLoaded += new EventHandler(Storage_ResourceCollectionLoaded);
            CurrentScheduler.DragOver += new System.Windows.Forms.DragEventHandler(CurrentScheduler_DragOver);
            CurrentScheduler.DragDrop += new System.Windows.Forms.DragEventHandler(CurrentScheduler_DragDrop);

            IMouseHandlerService oldMouseHandler = (IMouseHandlerService)CurrentScheduler.GetService(typeof(IMouseHandlerService));
            if(oldMouseHandler != null) {
                DragrDropMouseHandlerService newMouseHandler = new DragrDropMouseHandlerService(CurrentScheduler, oldMouseHandler);
                CurrentScheduler.RemoveService(typeof(IMouseHandlerService));
                CurrentScheduler.AddService(typeof(IMouseHandlerService), newMouseHandler);
            }
        }

        void CurrentScheduler_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
            if(e.Data.GetDataPresent(typeof(DevExpress.XtraScheduler.Resource))) {

                SchedulerHitInfo dropHitInfo = CurrentScheduler.ActiveView.ViewInfo.CalcHitInfo(CurrentScheduler.PointToClient(new Point(e.X, e.Y)), false);
                if(dropHitInfo.HitTest == SchedulerHitTest.ResourceHeader) {
                    Resource sourceResource = e.Data.GetData(typeof(DevExpress.XtraScheduler.Resource)) as DevExpress.XtraScheduler.Resource;
                    Resource targetResource = dropHitInfo.ViewInfo.Resource;
                    if(sourceResource != targetResource) {
                        object sourceResourceSortOrder = sourceResource.CustomFields[CustomFieldName];
                        sourceResource.CustomFields[CustomFieldName] = targetResource.CustomFields[CustomFieldName];
                        targetResource.CustomFields[CustomFieldName] = sourceResourceSortOrder;
                        ApplySorting();
                    }
                }
            }
        }

        void CurrentScheduler_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
            if(e.Data.GetDataPresent(typeof(DevExpress.XtraScheduler.Resource))) {
                e.Effect = DragDropEffects.Move;
            }
        }

        void ApplySorting() {
            CurrentScheduler.Storage.Resources.Items.Sort(new ResourceCustomFieldComparer("SortOrder"));
            CurrentScheduler.ActiveView.LayoutChanged();
        }


        void Storage_ResourceCollectionLoaded(object sender, EventArgs e) {
            ApplySorting();
        }
    }

    // MOUSE HANDLER SERVICE
    public class DragrDropMouseHandlerService : MouseHandlerServiceWrapper {
        IServiceProvider provider;
        SchedulerHitInfo downHitInfo;
        Point downHitPoint;


        public DragrDropMouseHandlerService(IServiceProvider provider, IMouseHandlerService service)
            : base(service) {
            this.provider = provider;
        }

        public override void OnMouseDown(MouseEventArgs e) {
            downHitInfo = null;
            downHitPoint = Point.Empty;
            SchedulerHitInfo hitInfo = (provider as SchedulerControl).ActiveView.ViewInfo.CalcHitInfo(e.Location, false);
            if(hitInfo.HitTest == SchedulerHitTest.ResourceHeader) {
                downHitInfo = hitInfo;
                downHitPoint = e.Location;
            }
            else 
                base.OnMouseDown(e);
        }

        public override void OnMouseMove(MouseEventArgs e) {
            if(e.Button == MouseButtons.Left && downHitInfo != null) {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(downHitPoint.X - dragSize.Width / 2, downHitPoint.Y - dragSize.Height / 2), dragSize);

                if(!dragRect.Contains(new Point(e.X, e.Y))) {
                    (provider as SchedulerControl).DoDragDrop(downHitInfo.ViewInfo.Resource, DragDropEffects.All);
                    downHitInfo = null;
                    downHitPoint = Point.Empty;
                }
            }
            else
                base.OnMouseMove(e);            
        }
    }



    // RESOURECE COMPARER
    public abstract class ResourceBaseComparer : IComparer<Resource>, IComparer {
        #region IComparer Members
        int IComparer.Compare(object x, object y) {
            return CompareCore(x, y);
        }
        public int Compare(Resource x, Resource y) {
            return CompareCore(x, y);
        }
        #endregion

        protected virtual int CompareCore(object x, object y) {
            Resource xRes = (Resource)x;
            Resource yRes = (Resource)y;

            if(xRes == null || yRes == null)
                return 0;
            if(Object.Equals(xRes.Id, ResourceEmpty.Id) || Object.Equals(yRes.Id, ResourceEmpty.Id))
                return 0;

            return CompareResources(xRes, yRes);
        }

        protected abstract int CompareResources(Resource xRes, Resource yRes);
    }

    public class ResourceCustomFieldComparer : ResourceBaseComparer {
        string customFieldValue = "";
        public ResourceCustomFieldComparer(string parCustomField) {
            customFieldValue = parCustomField;
        }

        protected override int CompareResources(Resource xRes, Resource yRes) {
            return Convert.ToInt32(xRes.CustomFields[customFieldValue]).CompareTo(Convert.ToInt32(yRes.CustomFields[customFieldValue]));
        }
    }
}
