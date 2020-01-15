namespace CNCalendarUWP
{
    public struct Lunar_t
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool Is_leap { get; set; }
        public Lunar_t(int Y, int M, int D,bool IsLeap)
        {
            Year = Y;
            Month = M;
            Day = D;
            Is_leap = IsLeap;
        }
    }

    class LunarToSolar
    {
        /*
          bit23 bit22 bit21 bit20:表示当年闰月月份，值为0 为则表示当年无闰月
          bit19 bit18 bit17 bit16 bit15 bit14 bit13 bit12 bit11 bit10 bit9 bit8 bit7
            1     2     3     4     5     6     7     8     9     10    11   12   13
          农历1-13 月大小 。月份对应位为1，农历月大(30 天),为0 表示小(29 天)
          bit6 bit5 bit4 bit3 bit2 bit1 bit0
          春节的公历月份      春节的公历日期
        */
        private static readonly int[] lunar200y = {            
            0x04AE53,0x0A5748,0x5526BD,0x0D2650,0x0D9544,0x46AAB9,0x056A4D,0x09AD42,0x24AEB6,0x04AE4A,/*1901-1910*/
            0x6A4DBE,0x0A4D52,0x0D2546,0x5D52BA,0x0B544E,0x0D6A43,0x296D37,0x095B4B,0x749BC1,0x049754,/*1911-1920*/
            0x0A4B48,0x5B25BC,0x06A550,0x06D445,0x4ADAB8,0x02B64D,0x095742,0x2497B7,0x04974A,0x664B3E,/*1921-1930*/
            0x0D4A51,0x0EA546,0x56D4BA,0x05AD4E,0x02B644,0x393738,0x092E4B,0x7C96BF,0x0C9553,0x0D4A48,/*1931-1940*/
            0x6DA53B,0x0B554F,0x056A45,0x4AADB9,0x025D4D,0x092D42,0x2C95B6,0x0A954A,0x7B4ABD,0x06CA51,/*1941-1950*/
            0x0B5546,0x555ABB,0x04DA4E,0x0A5B43,0x352BB8,0x052B4C,0x8A953F,0x0E9552,0x06AA48,0x6AD53C,/*1951-1960*/
            0x0AB54F,0x04B645,0x4A5739,0x0A574D,0x052642,0x3E9335,0x0D9549,0x75AABE,0x056A51,0x096D46,/*1961-1970*/
            0x54AEBB,0x04AD4F,0x0A4D43,0x4D26B7,0x0D254B,0x8D52BF,0x0B5452,0x0B6A47,0x696D3C,0x095B50,/*1971-1980*/
            0x049B45,0x4A4BB9,0x0A4B4D,0xAB25C2,0x06A554,0x06D449,0x6ADA3D,0x0AB651,0x093746,0x5497BB,/*1981-1990*/
            0x04974F,0x064B44,0x36A537,0x0EA54A,0x86B2BF,0x05AC53,0x0AB647,0x5936BC,0x092E50,0x0C9645,/*1991-2000*/
            0x4D4AB8,0x0D4A4C,0x0DA541,0x25AAB6,0x056A49,0x7AADBD,0x025D52,0x092D47,0x5C95BA,0x0A954E,/*2001-2010*/
            0x0B4A43,0x4B5537,0x0AD54A,0x955ABF,0x04BA53,0x0A5B48,0x652BBC,0x052B50,0x0A9345,0x474AB9,/*2011-2020*/
            0x06AA4C,0x0AD541,0x24DAB6,0x04B64A,0x69573D,0x0A4E51,0x0D2646,0x5E933A,0x0D534D,0x05AA43,/*2021-2030*/
            0x36B537,0x096D4B,0xB4AEBF,0x04AD53,0x0A4D48,0x6D25BC,0x0D254F,0x0D5244,0x5DAA38,0x0B5A4C,/*2031-2040*/
            0x056D41,0x24ADB6,0x049B4A,0x7A4BBE,0x0A4B51,0x0AA546,0x5B52BA,0x06D24E,0x0ADA42,0x355B37,/*2041-2050*/
            0x09374B,0x8497C1,0x049753,0x064B48,0x66A53C,0x0EA54F,0x06B244,0x4AB638,0x0AAE4C,0x092E42,/*2051-2060*/
            0x3C9735,0x0C9649,0x7D4ABD,0x0D4A51,0x0DA545,0x55AABA,0x056A4E,0x0A6D43,0x452EB7,0x052D4B,/*2061-2070*/
            0x8A95BF,0x0A9553,0x0B4A47,0x6B553B,0x0AD54F,0x055A45,0x4A5D38,0x0A5B4C,0x052B42,0x3A93B6,/*2071-2080*/
            0x069349,0x7729BD,0x06AA51,0x0AD546,0x54DABA,0x04B64E,0x0A5743,0x452738,0x0D264A,0x8E933E,/*2081-2090*/
            0x0D5252,0x0DAA47,0x66B53B,0x056D4F,0x04AE45,0x4A4EB9,0x0A4D4C,0x0D1541,0x2D92B5          /*2091-2099*/
        };
        private static readonly int MIN_YEAR = 1901;
        private static readonly int MAX_YEAR = 2099;
        private static readonly int[] MonthTotal = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        //判断公历的年份是否为闰年
        public static bool Is_gregorian_leap(int year)
        {
            return ((((year % 4) == 0) && ((year % 100) != 0)) || ((year % 400) == 0));
        }
        //判断农历的年份是否为闰年
        public static bool IsLunarLeapYear(int y)
        {
            if (((lunar200y[y - MIN_YEAR] & 0xf00000) >> 20) > 0)
                return true;
            return false;
        }
        //0x04AE53
        //
        //0000 0100 1010 1110 0101 0011
        //返回农历某年的闰月月份
        public static int LunarLeapMonth(int y)
        {
            return ((lunar200y[y - MIN_YEAR] & 0xf00000) >> 20);
        }
        //返回农历某年某月的天数
        public static int LunarMonthDays(int y, int m)
        {
            if ((lunar200y[y - MIN_YEAR] & (0x80000 >> (m - 1))) > 0)
                return 30;
            return 29;
        }
        //返回农历某年的闰月的天数
        public static int LunarLeapMonthDays(int y)
        {
            if (LunarLeapMonth(y) > 0)
            {
                if ((lunar200y[y - MIN_YEAR] & (0x80000 >> (LunarLeapMonth(y) - 1))) > 0)
                    return 30;
                return 29;
            }
            else
                return 0;
        }
        //计算某年的春节的公历日期
        public static void LunarSpringDate(int y,ref int solar_m,ref int solar_d)
        {
            solar_m = (lunar200y[y - MIN_YEAR] & 0x0060) >> 5;
            solar_d =  lunar200y[y - MIN_YEAR] & 0x1f ;
        }
        //判断给定的农历日期是否是，在MIN_YEAR年到MAX_YEAR年之间正确的日期
        private static bool LunarDateCheck(Lunar_t lunar)
        {
            int leap = 0;
            if ((lunar.Year < MIN_YEAR) || (lunar.Year > MAX_YEAR))
            {
                return false;
            }
            if ((lunar.Month < 1) || (lunar.Month > 12))
            {
                return false;
            }
            if ((lunar.Day < 1) || (lunar.Day > 30)) //农历的月最多有30天
            {
                return false;
            }
            if (LunarMonthDays(lunar.Year, lunar.Month) < lunar.Day)
            {
                return false;
            }
            if (lunar.Is_leap)
            {
                leap = LunarLeapMonth(lunar.Year);
                if (leap == 0)
                    return false;
                else if (lunar.Month != leap)
                    return false;
                else if (LunarLeapMonthDays(lunar.Year) < lunar.Day)
                {
                    return false;
                }
            }
            return true;
        }
        //判断给定的公历日期是否是，在MIN_YEAR年到MAX_YEAR年之间正确的日期
        private static bool SolarDateCheck(int year, int month, int day)
        {
            if ((year < MIN_YEAR) || (year > MAX_YEAR))
            {
                return false;
            }
            if ((month < 1) || (month > 12))
            {
                return false;
            }
            if ((day < 1) || (day > 31))
            {
                return false;
            }
            if (month == 2)
            {
                if (Is_gregorian_leap(year))// 是闰年
                {
                    if (day > 29)
                        return false;
                }
                else
                {
                    if (day > 28)
                        return false;
                }
            }
            return true;
        }
        //农历转公历
        
        public static bool ToSolar(Lunar_t lunar, ref int y, ref int m, ref int d)
        {
            
            int year = lunar.Year,
            month = lunar.Month,
            day = lunar.Day;
            int byNow, xMonth, i;
            int spring_solar_m = 0;
            int spring_solar_d = 0;
            if (!LunarDateCheck(lunar))
                return false;
            LunarSpringDate(year,ref spring_solar_m,ref spring_solar_d);
            byNow = spring_solar_d - 1;
            if (spring_solar_m == 2)
                byNow += 31;
            for (i = 1; i < month; i++)
            {
                byNow += LunarMonthDays(year, i);
            }
            byNow += day;
            xMonth = LunarLeapMonth(year);
            if (xMonth != 0)
            {
                if (month > xMonth || (month == xMonth && lunar.Is_leap))
                {
                    byNow += LunarLeapMonthDays(year);
                }
            }
            if (byNow > 366 || (year % 4 != 0 && byNow == 365))
            {
                year += 1;
                if (year % 4 == 0)
                    byNow -= 366;
                else
                    byNow -= 365;
            }
            for (i = 1; i < 13; i++)   //这行代码可能有错
            {
                if (MonthTotal[i] >= byNow)
                {
                    month = i;
                    break;
                }
            }
            d =  byNow - MonthTotal[month - 1] ;
            m =  month ;
            y = year;
            return true;
        }
        
        
        //公历转农历
        public static bool ToLunar(int year, int month, int day, ref Lunar_t lunar)
        {
            int bySpring, bySolar, daysPerMonth;
            int index = 0;
            bool flag;
            int spring_solar_m = 0;
            int spring_solar_d = 0;
            if (!SolarDateCheck(year, month, day) )
                return false;
            //bySpring 记录春节离当年元旦的天数。
            //bySolar 记录阳历日离当年元旦的天数。
            LunarSpringDate(year, ref spring_solar_m, ref spring_solar_d);
            if (spring_solar_m == 1)
                bySpring = spring_solar_d - 1;
            else
                bySpring = spring_solar_d - 1 + 31;
            bySolar = MonthTotal[month - 1] + day - 1;
            if ((year % 4 == 0) && (month > 2))
                bySolar++;
            //daysPerMonth记录大小月的天数 29 或30
            //index 记录从哪个月开始来计算。
            //flag 是用来对闰月的特殊处理。
            //判断阳历日在春节前还是春节后
            if (bySolar >= bySpring) //阳历日在春节后（含春节那天）
            {
                bySolar -= bySpring;
                month = 1;
                flag = false;

                daysPerMonth = LunarMonthDays(year, month);
                while (bySolar >= daysPerMonth)
                {
                    bySolar -= daysPerMonth;
                    if (month == LunarLeapMonth(year))
                    {
                        flag = !flag;

                        if (!flag)
                        {
                            month++;
                        }
                    }
                    else
                    {
                        month++;
                    }
                    index++;
                    daysPerMonth = LunarMonthDays(year, index);
                }
                day = bySolar + 1;

            }
            else
            {                                                      //阳历日在春节前 
                bySpring -= bySolar;
                year--;
                month = 12;
                if (LunarLeapMonth(year) == 0)
                    index = 12;
                else
                    index = 13;
                flag = false;
                daysPerMonth = LunarMonthDays(year, index);
                while (bySpring > daysPerMonth)
                {
                    bySpring -= daysPerMonth;
                    index--;
                    if (!flag)
                        month--;
                    if (month == LunarLeapMonth(year))
                    {
                        flag = !flag;
                    }
                    
                    daysPerMonth = LunarMonthDays(year, index);
                }
                day = daysPerMonth - bySpring + 1;
            }
            lunar.Day = day;
            lunar.Month = month;
            lunar.Year = year;
            if (month == LunarLeapMonth(year))
                lunar.Is_leap = true;
            else
                lunar.Is_leap = false;
            return true;
           
        }
        
    }
}
