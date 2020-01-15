using System;
using System.Collections.Generic;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Text;
using System.ServiceModel.Channels;

namespace CNCalendarUWP
{
    public class Node
    {
        public Schedule data;
        public Node next;
    }//Node

    public class MyList
    {//单向循环链
        private Node tail;//链尾附加结点
        private int m_count = 0;//除链尾附加结点外，结点总数
        public int Count { get { return m_count; } }
        private readonly string DataFolderName = "DataFolder";
        private readonly string FileNmae = "Filename.dat";
        //索引器
        public Schedule this[int i]
        {
            get {
                Node p;
                p = tail.next;
                int n = 0;
                while (p != tail && n<i)
                {
                    p = p.next;
                    n++;
                }   //	while

                return p.data;
            }
        }
        public MyList()
        {
            tail = new Node();
            tail.next = tail;
        }
        public MyList(Schedule s)
        {
            tail = new Node();
            tail.next = tail;
            Add(s);
        }

        public bool Add(Schedule s)
        {
            Node p, q;
            p = new Node();
            q = tail.next;
            tail.data = s;
            tail.next = p;
            tail = p;
            tail.next = q;
            m_count++;
            return true;  // ok
        }
        public MyList FindAll(Schedule x)
        {
            Node p;
            p = tail.next;
            MyList list = new MyList();
            while (p != tail)
            {
                if (x.Compare(p.data,x) == 0)//p.data与x相等
                {
                    list.Add(p.data);
                }
                p = p.next;
            }   //	while
            return list;
        }//Search

        public void Sort()
        {
            Node p1, q1, p2, q2;
            q1 = tail.next;//q1为旧表的第一个结点
            p1 = q1.next;//p1为旧表的第二个结点
            tail.next = null;//新表初始化为空
            while (q1 != tail)
            {
                q2 = tail;
                p2 = q2.next;//p2为新表第一个结点
                while (p2 != null && p2.data.Compare(p2.data,q1.data) < 0)
                {
                    q2 = q2.next;
                    p2 = q2.next;
                }//while
                 //把q1插入到q2和p2之间
                q2.next = q1;
                q1.next = p2;

                q1 = p1;
                p1 = p1.next;
            }//while
            q2 = tail;
            p2 = q2.next;
            //找到新表的最后一个结点
            while (p2 != null)
            {
                q2 = q2.next;
                p2 = q2.next;
            }//while
            q2.next = tail;//把新表的最后一个结点链接到tail
        }//Sort

        //
        // 摘要:
        //     创建一个与指定的 MyList 具有相同值的 MyList 的新实例。
        //
        // 参数:
        //   x:
        //     要复制的 MyList 的实例。
        //
        // 返回结果:
        //     值与 x 相同的MyList的新实例。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     x 为 null。
        public MyList Copy(MyList x)
        {
            try
            {
                Node p;
                p = x.tail.next;
                MyList tempList = new MyList();
                while (p != x.tail)
                {
                    tempList.Add(p.data);
                    p = p.next;
                }
                return tempList;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void Clear()
        {
            Node p, q;
            p = tail.next;
            q = p;
            
            while (p != tail)
            {
                q = q.next;
                p = q;
                m_count--;
            }//while
            tail.next = tail;
        }//Clear

        public bool Delete(Schedule x)
        {
            Node p, q;
            q = tail;
            p = q.next;
            while (p != tail && x.Compare(p.data,x) != 0)
            {
                q = p;
                p = p.next;
            }   //	while
            if (p == tail)
            {
                return false;   //如果待删除的元素不存在链表中，则返回false
            }   //	if
            else
            {
                q.next = p.next;
                m_count--;
                return true; 
            }   //	else
        }//Delete

        //写入文件
        public async Task WriteAsync()
        {
            try
            {
                //获取本地文件夹根目录
                StorageFolder local = ApplicationData.Current.LocalFolder;
                StorageFolder dataFolder = await local.CreateFolderAsync(DataFolderName, CreationCollisionOption.OpenIfExists);
                StorageFile file = await dataFolder.CreateFileAsync(FileNmae, CreationCollisionOption.ReplaceExisting);
                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                {
                    using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                    {
                        Node p;
                        p = tail.next;

                        while (p != tail)
                        {
                            //写入日程日期
                            long date = p.data.Date.ToBinary();
                            dataWriter.WriteInt64(date);
                            //写入日程标题
                            dataWriter.WriteInt32(p.data.Title.Length);
                            dataWriter.WriteString(p.data.Title.ToString());
                            //写入日程内容
                            dataWriter.WriteInt32(p.data.Body.Length);
                            dataWriter.WriteString(p.data.Body.ToString());
                            p = p.next;
                        }//while
                    }//using
                }//using
              
            }//try
            catch (Exception e)
            {
                Message.CreateMessage(MessageVersion.Default, e.Message);
            }//catch

        }//WriteAsync


        //读取文件
        public async Task ReadAsync()
        {            
            try
            {
                Clear();
                //获取本地文件夹根目录
                StorageFolder local = ApplicationData.Current.LocalFolder;
                StorageFolder dataFolder = await local.CreateFolderAsync(DataFolderName, CreationCollisionOption.OpenIfExists);
                StorageFile file = await dataFolder.CreateFileAsync(FileNmae, CreationCollisionOption.OpenIfExists);
                using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (DataReader dataReader = new DataReader(readStream))
                    {
                        Node p = new Node();
                        uint x =  await dataReader.LoadAsync(sizeof(long));
                        while (x != 0)
                        {
                            //读入日程日期
                            long BinDate = dataReader.ReadInt64();
                            p.data.Date = new DateTime(BinDate);
                            //读入日程标题
                            await dataReader.LoadAsync(sizeof(int));
                            int titleSize = dataReader.ReadInt32();
                            await dataReader.LoadAsync((uint)titleSize);
                            string title = dataReader.ReadString((uint)titleSize);
                            p.data.Title = title;
                            //读入日程内容
                            await dataReader.LoadAsync(sizeof(int));
                            int bodySize = dataReader.ReadInt32();
                            await dataReader.LoadAsync((uint)bodySize);
                            string body = dataReader.ReadString((uint)titleSize);
                            p.data.Body = body;
                            //加载下一个日程的日期
                            x = await dataReader.LoadAsync(sizeof(long));
                        }//while
                    }//using
                }//using
            }//try
            catch (Exception e)
            {
                Message.CreateMessage(MessageVersion.Default, e.Message);
            }//catch
        }//ReadAsync


    }//MyList

}//Link_Test
