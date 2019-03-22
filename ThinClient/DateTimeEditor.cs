using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Popups.Calendar;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows;
using System.Windows.Controls;

namespace ThinClient
{
    public class DateTimeEditCalendar : DateEditCalendar
    {
        public DateTimeEditCalendar()
        {
            //
        }

        protected override void OnDateTimeChanged()
        {
            base.OnDateTimeChanged();
            Time = DateTime;
        }

        protected override void OnDayCellButtonClick(Button button)
        {
            if (OwnerDateEdit == null)
            {
                if (button != null)
                {
                    DateTime = (DateTime)DateEditCalendar.GetDateTime(button);
                }
                return;
            }
            if (!OwnerDateEdit.IsReadOnly)
            {
                var newDate = (DateTime)DateEditCalendar.GetDateTime(button);
                PostDateTimeToEditValue(CombineDateTime(newDate, Time));
            }
            OwnerDateEdit.IsPopupOpen = false;
        }

        public DateTime Time
        {
            get { return ((DateTime)GetValue(TimeProperty)); }
            set { SetValue(TimeProperty, value); }
        }

        public static DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(DateTime), typeof(DateTimeEditCalendar),
                                        new PropertyMetadata(new DateTime(),
                                                             new PropertyChangedCallback(
                                                                 (obj, e) =>
                                                                 {
                                                                     var calendar = (DateTimeEditCalendar)obj;
                                                                     calendar.PostDateTimeToEditValue(calendar.CombineDateTime(calendar.DateTime, (DateTime)e.NewValue));
                                                                 })));

        void PostDateTimeToEditValue(DateTime dt)
        {
            ActualPropertyProvider.GetProperties(OwnerDateEdit).GetService<ValueContainerService>().SetEditValue(dt, UpdateEditorSource.ValueChanging);
        }

        DateTime CombineDateTime(DateTime date, DateTime time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }
}
