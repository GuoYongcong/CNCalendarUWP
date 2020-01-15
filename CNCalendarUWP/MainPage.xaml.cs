//程序名：MainPage.xaml.cs
//      程序功能：
//          作者：郭永聪
//          日期：2018.10.30
//          版本：2.0
//      修改内容：添加点击日期按钮，页面更新显示日期按钮对应的日期的功能
//      修改日期：2018.11.2
//      修改作者：郭永聪
//
//

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace CNCalendarUWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Button[,] buttonArray;//二维数组存储42个按钮
        private int[,] dayInButton;//存储42个按钮显示的天数
        private List<Button> list;//存储没有显示日期的按钮
        private Brush brushBrown;//棕色
        private Brush brushWheat;//小麦色
        private Brush brushBlack;//黑色
        private Brush brushTransparent;//透明
        private Button priClickButton;//上一次点击的显示日期的按钮
        private Button priCickMenuButton;//上一次点击的导航按钮
        private Button ButtonTodayTemp;//显示当天的按钮
        private string[] datas100Years;//公历100年
        private string[] datas28Days;//公历二月28天
        private string[] datas29Days;//公历润二月29天
        private string[] datas30Days;//公历小月30天
        private string[] datas31Days;//公历大月31天
        private string[] datas29LunarDays;//农历小月29天
        private string[] datas30LunarDays;//农历大月30天

        public MainPage()
        {
            InitializeComponent();
            buttonArray = new Button[,]{
                {button00,button01,button02,button03,button04,button05,button06 },
                {button10,button11,button12,button13,button14,button15,button16 },
                {button20,button21,button22,button23,button24,button25,button26 },
                {button30,button31,button32,button33,button34,button35,button36 },
                {button40,button41,button42,button43,button44,button45,button46 },
                {button50,button51,button52,button53,button54,button55,button56 }
            };

            brushBrown = buttonBackToToday.Background;
            brushWheat = buttonBackToToday.Foreground;
            brushBlack = button00.Foreground;
            brushTransparent = button00.BorderBrush;

            priCickMenuButton = MonthButton;//月视图按钮

            string[] firstWord = { "初", "十", "廿" };
            string[] secondWord = { "十", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            datas29LunarDays = new string[29];
            datas30LunarDays = new string[30];

            for (int i = 1; i < 30; i++)
            {
                if (i / 10 == 1 && i % 10 == 0)
                    datas30LunarDays[i - 1] = datas29LunarDays[i - 1] = "初十";
                else
                {
                    if (i / 10 == 2 && i % 10 == 0)
                        datas30LunarDays[i - 1] = datas29LunarDays[i - 1] = "二十";
                    else
                        datas30LunarDays[i - 1] = datas29LunarDays[i - 1] = firstWord[i / 10] + secondWord[i % 10];
                }
            }
            datas30LunarDays[29] = "三十";

            datas100Years = new string[100];
            for (int i = 0; i < 100; i++)
            {
                datas100Years[i] = $"{i + 1970}年";
            }
            datas28Days = new string[28];
            datas29Days = new string[29];
            datas30Days = new string[30];
            datas31Days = new string[31];
            string[] str = new string[] {"0","",""};

            for (int i = 0; i < 28; i++)
            {
                string s = str[(i + 1) / 10];
                datas28Days[i] = datas29Days[i] = datas30Days[i] = datas31Days[i] = s + $"{i + 1}日";
            }
            datas29Days[28] = datas30Days[28] = datas31Days[28] = "29日";
            datas30Days[29] = datas31Days[29] = "30日";
            datas31Days[30] = "31日";

            comboBoxYears.DropDownClosed += ComboBoxDropDownClosed_Solar;
            comboBoxMonths.DropDownClosed += ComboBoxDropDownClosed_Solar;
            comboBoxDays.DropDownClosed += ComboBoxDropDownClosed_Solar;

            comboBoxLunarYears.DropDownClosed += ComboBoxDropDownClosed_Lunar;
            comboBoxLunarMonths.DropDownClosed += ComboBoxDropDownClosed_Lunar;
            comboBoxLunarDays.DropDownClosed += ComboBoxDropDownClosed_Lunar;

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            Combobox_loaded(currentTime);
            TextBlock_loaded();
            //星期六、日的字体颜色为brown
            for (int i = 0; i < 6; i++)
            {
                buttonArray[i, 6].Foreground = brushBrown;//星期六
                buttonArray[i, 0].Foreground = brushBrown;//星期日
            }

            DateTime dateTime = new DateTime(currentTime.Year, currentTime.Month, 1);
            int weekday = (int)dateTime.DayOfWeek;
            int number = 0;
            dayInButton = new int[6,7];
            /*
            while (weekday < 7)
            {
                if (number == currentTime.Day)
                    ButtonTodayTemp = buttonArray[0, weekday];
                string lunarDate = LunarCalendar.GetLunarDay(currentTime.Year, currentTime.Month, number, false);
                string Blank1 = GetBlank(lunarDate.Length);
                buttonArray[0, weekday].Content = Blank1 + $" {number}\n" + lunarDate;
                dayInButton[0, weekday] = number;
                weekday++;
                number++;
            }
            */
            
            int maxDay = DateTime.DaysInMonth(currentTime.Year, currentTime.Month);
            list = new List<Button>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    number = i * 7 + j + 1 - weekday;
                    if (number > 0 && number <= maxDay )
                    {
                        if (number == currentTime.Day)
                            ButtonTodayTemp = buttonArray[i, j];
                        string lunarDate = LunarCalendar.GetLunarDay(currentTime.Year, currentTime.Month, number, false);

                        string Blank2 = GetBlank(lunarDate.Length);

                        if (number < 10)
                        {
                            buttonArray[i, j].Content = Blank2 + $" {number}\n" + lunarDate;
                        }
                        else
                        {
                            buttonArray[i, j].Content = Blank2 + $"{number}\n" + lunarDate;
                        }
                        dayInButton[i, j] = number;
                    }
                    else
                    {
                        buttonArray[i, j].IsEnabled = false;//使按钮失效
                        list.Add(buttonArray[i, j]);
                        dayInButton[i, j] = 0;
                    }
                }
            }
            if (ButtonTodayTemp != null)
            {
                ButtonTodayTemp.Background = brushBrown;
                ButtonTodayTemp.Foreground = brushWheat;
            }
        }

        public void Combobox_loaded(DateTime currentTime)
        {
            comboBoxYears.ItemsSource = datas100Years;
            int year = currentTime.Year;
            comboBoxYears.SelectedItem = $"{year}年";
            comboBoxMonths.SelectedItem = $"{currentTime.Month}月";
            int daysOfSolarMonth = DaysOfSolarMonth(currentTime.Year, currentTime.Month);
            switch (daysOfSolarMonth)
            {
                case 28: comboBoxDays.ItemsSource = datas28Days; break;
                case 29: comboBoxDays.ItemsSource = datas29Days; break;
                case 30: comboBoxDays.ItemsSource = datas30Days; break;
                case 31: comboBoxDays.ItemsSource = datas31Days; break;
                default: break;
            }
            comboBoxDays.SelectedIndex = currentTime.Day - 1;

            comboBoxLunarYears.ItemsSource = datas100Years;
            comboBoxLunarYears.SelectedItem = LunarCalendar.GetLunarYear(currentTime.Year, currentTime.Month, currentTime.Day) + "年";
            string LunarDay = LunarCalendar.GetLunarDay(currentTime.Year, currentTime.Month, currentTime.Day, false);
            comboBoxLunarMonths.SelectedItem = LunarCalendar.GetStringLunarMonth(currentTime.Year, currentTime.Month, currentTime.Day);

            int lunarMonthDays = LunarCalendar.GetIntLunarMonth(currentTime.Year, currentTime.Month, currentTime.Day)[1];
            if (lunarMonthDays == 29)
                comboBoxLunarDays.ItemsSource = datas29LunarDays;
            else
                comboBoxLunarDays.ItemsSource = datas30LunarDays;
            comboBoxLunarDays.SelectedItem = LunarCalendar.GetLunarDay(currentTime.Year, currentTime.Month, currentTime.Day, true);

        }

        public void TextBlock_loaded()
        {
            try
            {
                textBlockYear.Text = (string)comboBoxYears.SelectedItem;
                textBlockMonth.Text = (string)comboBoxMonths.SelectedItem;
                textBlockDay.Text = (string)comboBoxDays.SelectedItem;
                textBlockLunarMonth.Text = (string)comboBoxLunarMonths.SelectedItem;
                textBlockLunarDay.Text = (string)comboBoxLunarDays.SelectedItem;
            } catch (Exception e){}
            
        }

        //刷新页面函数
        public void Refresh(DateTime dateTime)
        {
            foreach (Button button in list)
            {
                button.IsEnabled = true;//使按钮能点击
            }
            list.Clear();//清空链表
            for (int i = 0; i < 7; i++)
            {
                buttonArray[0, i].Content = "";
                buttonArray[4, i].Content = "";
                buttonArray[5, i].Content = "";
            }
            ButtonTodayTemp.Background = brushWheat;
            ButtonTodayTemp.Foreground = brushBlack;
            if (priClickButton != null)
                priClickButton.BorderBrush = brushTransparent;

            //星期六、日的字体颜色为brown
            for (int i = 0; i < 6; i++)
            {
                buttonArray[i, 6].Foreground = brushBrown;//星期六
                buttonArray[i, 0].Foreground = brushBrown;//星期日
            }

            DateTime date = new DateTime(dateTime.Year, dateTime.Month, 1);
            int weekday = (int)date.DayOfWeek;
            int number = 0;
            int maxDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    number = i * 7 + j + 1 - weekday;
                    if (number > 0 && number <= maxDay)
                    {
                        if (number == dateTime.Day)
                            priClickButton = buttonArray[i, j];

                        string lunarDate = LunarCalendar.GetLunarDay(dateTime.Year, dateTime.Month, number, false);
                        string Blank2 = GetBlank(lunarDate.Length);

                        if (number < 10)
                        {
                            buttonArray[i, j].Content = Blank2 + $" {number}\n" + lunarDate;
                        }
                        else
                        {
                            buttonArray[i, j].Content = Blank2 + $"{number}\n" + lunarDate;
                        }
                        dayInButton[i, j] = number;
                        number++;
                    }
                    else
                    {
                        buttonArray[i, j].IsEnabled = false;//使按钮失效
                        list.Add(buttonArray[i, j]);
                        dayInButton[i, j] = 0;
                    }

                }
            }
            priClickButton.BorderBrush = brushBrown;
            DateTime Today = DateTime.Now;
            if (dateTime.Year == Today.Year && dateTime.Month == Today.Month)
            {
                ButtonTodayTemp.Background = brushBrown;
                ButtonTodayTemp.Foreground = brushWheat;
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            if (priClickButton != null)
            {
                priClickButton.BorderBrush = brushTransparent;
            }
            b.BorderBrush = brushBrown;
            priClickButton = b;
            int i = b.Name[b.Name.Length - 2]-'0';
            int j = b.Name[b.Name.Length - 1]-'0';
            int d = dayInButton[i,j];//该按钮显示的天数
            //更新显示的日期
            comboBoxDays.SelectedIndex = d - 1;
            int solarYear = comboBoxYears.SelectedIndex + 1970;
            int solarMonth = comboBoxMonths.SelectedIndex + 1;
            int solarDay = comboBoxDays.SelectedIndex + 1;
            DateTime dateTime = new DateTime(solarYear, solarMonth, solarDay);
            int lunarMonthDays = LunarCalendar.DaysOfLunarMonth(solarYear, solarMonth);

            if (lunarMonthDays == 29)
            {
                comboBoxLunarDays.ItemsSource = datas29LunarDays;
            }//if
            else
            {
                comboBoxLunarDays.ItemsSource = datas30LunarDays;
            }//else
            int lunarYear = LunarCalendar.GetLunarYear(solarYear, solarMonth, solarDay);
            string lunarMonth = LunarCalendar.GetStringLunarMonth(solarYear, solarMonth, solarDay);
            int lunarDay = LunarCalendar.GetIntLunarDay(solarYear, solarMonth, solarDay);

            comboBoxLunarYears.SelectedItem = $"{lunarYear}年";
            comboBoxLunarMonths.SelectedItem = lunarMonth;
            comboBoxLunarDays.SelectedIndex = lunarDay - 1;
            TextBlock_loaded();

        }

        private void ComboBoxDropDownClosed_Solar(object sender, object e)
        {
            int solarYear = comboBoxYears.SelectedIndex + 1970;
            int solarMonth = comboBoxMonths.SelectedIndex + 1;
            int solarDay = comboBoxDays.SelectedIndex + 1;
            DateTime dateTime = new DateTime(solarYear, solarMonth,solarDay);
            Refresh(dateTime);

            int lunarMonthDays = LunarCalendar.DaysOfLunarMonth(solarYear, solarMonth);

            if (lunarMonthDays == 29)
            {
                comboBoxLunarDays.ItemsSource = datas29LunarDays;
            }//if
            else
            {
                comboBoxLunarDays.ItemsSource = datas30LunarDays;
            }//else
            int daysOfSolarMonth = DaysOfSolarMonth(solarYear, solarMonth);
            switch (daysOfSolarMonth)
            {
                case 28: comboBoxDays.ItemsSource = datas28Days; break;
                case 29: comboBoxDays.ItemsSource = datas29Days; break;
                case 30: comboBoxDays.ItemsSource = datas30Days; break;
                case 31: comboBoxDays.ItemsSource = datas31Days; break;
                default: break;
            }
            
            comboBoxDays.SelectedIndex = solarDay - 1;

            int lunarYear = LunarCalendar.GetLunarYear(solarYear, solarMonth, solarDay);
            string lunarMonth = LunarCalendar.GetStringLunarMonth(solarYear, solarMonth, solarDay);
            int lunarDay = LunarCalendar.GetIntLunarDay(solarYear, solarMonth, solarDay);

            comboBoxLunarYears.SelectedItem = $"{lunarYear}年";
            comboBoxLunarMonths.SelectedItem = lunarMonth;
            comboBoxLunarDays.SelectedIndex = lunarDay - 1;
            TextBlock_loaded();
        }

        private void ComboBoxDropDownClosed_Lunar(object sender, object e)
        {
            int lunarYear = comboBoxLunarYears.SelectedIndex + 1970;
            int lunarMonth = comboBoxLunarMonths.SelectedIndex + 1;
            int lunarDay = comboBoxLunarDays.SelectedIndex + 1;
            bool isLeap = LunarToSolar.IsLunarLeapYear(lunarYear);
            Lunar_t lunar_T=new Lunar_t(lunarYear,lunarMonth,lunarDay,isLeap);
            
            int solarYear = 0;
            int solarMonth= 0;
            int solarDay = 0;
            
            if (LunarToSolar.ToSolar(lunar_T, ref solarYear, ref solarMonth, ref solarDay))
            {
                int daysOfSolarMonth = DaysOfSolarMonth(solarYear, solarMonth);
                switch (daysOfSolarMonth)
                {
                    case 28: comboBoxDays.ItemsSource = datas28Days; break;
                    case 29: comboBoxDays.ItemsSource = datas29Days; break;
                    case 30: comboBoxDays.ItemsSource = datas30Days; break;
                    case 31: comboBoxDays.ItemsSource = datas31Days; break;
                    default: break;
                }
                
                comboBoxYears.SelectedIndex = solarYear - 1970;
                comboBoxMonths.SelectedIndex = solarMonth - 1;
                comboBoxDays.SelectedIndex = solarDay - 1;
                
                int lunarMonthDays = LunarCalendar.DaysOfLunarMonth(solarYear, solarMonth);

                if (lunarMonthDays == 29)
                {
                    comboBoxLunarDays.ItemsSource = datas29LunarDays;
                }//if
                else
                {
                    comboBoxLunarDays.ItemsSource = datas30LunarDays;
                }//else
                if (lunarDay > lunarMonthDays)
                {
                    comboBoxLunarDays.SelectedIndex = lunarMonthDays - 1;
                }//if
                else
                {
                    comboBoxLunarDays.SelectedIndex = lunarDay - 1;
                }//else
                DateTime dateTime = new DateTime(solarYear, solarMonth, solarDay);
                Refresh(dateTime);
                TextBlock_loaded();
            }


        }

        public static string GetBlank(int sum)
        {
            string Blank = "";
            switch (sum)
            {
                case 3: sum++; break;
                case 4: sum += 2; break;
                case 5: sum += 3; break;
                case 6: 
                case 7: sum += 4; break;
                case 8: sum += 5; break;
                default:break;
            }
            for (int i = 0; i < sum; i++)
                Blank += " ";
            return Blank;

        }
        public static int DaysOfSolarMonth(int solarYear, int solarMonth)
        {
            switch (solarMonth)
            {
                case 1: case 3: case 5: case 7:case 8:case 10:
                case 12:return 31;
                case 4: case 6: case 9:
                case 11:return 30;
                case 2:
                    if ((solarYear % 4 == 0 && solarYear % 100 != 0) || solarYear % 400 == 0)
                        return 29;
                    return 28;
                default:return 0;
            }

        }
        private void ButtonBackToToday_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            Refresh(currentTime);
            Combobox_loaded(currentTime);
            TextBlock_loaded();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void JumpDateButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenuColor(JumpDateButton);

            if (MySplitView.IsPaneOpen)
            {
                MySplitView.IsPaneOpen = false;
            }

            JumpDateSplitView.IsPaneOpen = !JumpDateSplitView.IsPaneOpen;
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenuColor(MonthButton);

            if (MySplitView.IsPaneOpen)
            {
                MySplitView.IsPaneOpen = false;
            }
        }
        
        private void WeekButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenuColor(WeekButton);

            if (MySplitView.IsPaneOpen)
            {
                MySplitView.IsPaneOpen = false;
            }

        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenuColor(DayButton);

            if (MySplitView.IsPaneOpen)
            {
                MySplitView.IsPaneOpen = false;
            }

        }
        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenuColor(ScheduleButton);

            if (MySplitView.IsPaneOpen)
            {
                MySplitView.IsPaneOpen = false;
            }

        }
        

        private void MyToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            bool isOn = MyToggleSwitch.IsOn;
            //off:公历
            comboBoxYears.IsEnabled = !isOn;
            comboBoxMonths.IsEnabled = !isOn;
            comboBoxDays.IsEnabled = !isOn;
            //on:农历
            comboBoxLunarYears.IsEnabled = isOn;
            comboBoxLunarMonths.IsEnabled = isOn;
            comboBoxLunarDays.IsEnabled = isOn;
        }

        private void ChangeMenuColor(Button button)
        {
            priCickMenuButton.Background = brushWheat;
            priCickMenuButton.Foreground = brushBrown;

            priCickMenuButton = button;
            button.Background = brushBrown;
            button.Foreground = brushWheat;
        }
    }
}
