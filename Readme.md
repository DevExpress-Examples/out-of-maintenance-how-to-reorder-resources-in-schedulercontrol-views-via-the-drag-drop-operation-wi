<!-- default file list -->
*Files to look at*:

* [DragDropResourcesHelper.cs](./CS/WindowsFormsApplication1/DragDropResourcesHelper.cs) (VB: [DragDropResourcesHelper.vb](./VB/WindowsFormsApplication1/DragDropResourcesHelper.vb))
* [Form1.cs](./CS/WindowsFormsApplication1/Form1.cs) (VB: [Form1.vb](./VB/WindowsFormsApplication1/Form1.vb))
<!-- default file list end -->
# How to reorder resources in SchedulerControl views via the "drag-drop" operation (WinForms version)


<p>The main idea of this approach is to interchange a value of a custom field between the "dragged" and "dropped" resources.<br />An approach to sort (reorder) resources based on a custom field value was demonstrated in the following example:<br /><a href="https://www.devexpress.com/Support/Center/p/E3124">How to sort resources</a><br /><br />To build a solution demonstrated in this sample into an existing application, copy the "<strong>DragDropResourcesHelper</strong>" module and pass a current SchedulerControl instance and the "sort" field name into the <strong>DragDropResourcesHelper </strong>class constructor:<br /><br /></p>


```cs
DragDropResourcesHelper ddHelper = new DragDropResourcesHelper(schedulerControl1, "SortOrder");
```




```vb
Dim ddHelper As New DragDropResourcesHelper(schedulerControl1, "SortOrder")
```



<br/>


