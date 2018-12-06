
### # WeavingDB


#### 项目介绍
为满足C#项目的特殊使用与简单部署，而开发此WeavingDB。


- 1.支持K-V方式。
- 2.支持JSON数据的条件查询的内存缓存库。（目前JSON库部分，创建，清空，插入，批量插入，查询，有条件删除，修改）
- 3.可以设置最后一次访问时间的过期时效，例如：A数据，最后一次访问是1小时以前，设置过期为60分钟，此时A数据将被清理，如果A数据在60分钟内再次被访问则不清理。也可以设置永久不过期。
- 4.JSON库，需要先创建库，然后就可以插入数据了，数据库表的列为自动适应。第一次插入的数据即为默认列，以后插入的数据，可以增加字段，也可以减少字段(不需要重新建立表)。但是字段类型不能更改，如果原本的列名的类型改变，会引起类型错误。如果要改变原有字段类型，只能清空库后，从新建立库。
- 5.暂不支持数据持久化，暂不支持主从部署，当我想到一个高效模型，我会更新这两类内容。
- 6.数据通信部分使用WeavingSocket架构，通信部分架构地址：https://gitee.com/dreamsfly900/universal-Data-Communication-System-for-windows

 
#### 使用说明

K-V读写使用代码
```
 DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
           dbc.open();
           dbc.Set<String>("asdasd", "1");//设置
            int i = 0;
            String str2 = dbc.Get<String>("asdasd");//读取
 string [] keys=   dbc.GetKey("as?asd");//通配符?一个匹配字符
            keys = dbc.GetKey("as*");//通配符* 表示，多个模糊匹配
            dbc.close();
```

JSONDB，创建库，插入表，批量插入表，修改表，删除表，清空库 操作

```
  dbc.open();
            user u = new user();
            bool bbc = dbc.inserttable<user>("ddd", u);
            dbc.Createtable("ddd");//创建库

            bbc = dbc.inserttable<user>("ddd", u);//插入对象


            //每次插入一组数据
            List<user> list = new List<user>();

            int i = 0;
            while (i < 10000)
            {
                u = new user();
                u.id = i;
                list.Add(u);
                i++;
            }
            DateTime dt = DateTime.Now;
            bbc = dbc.inserttable<user>("ddd", list.ToArray());//批量插入库
        
            DateTime dt2 = DateTime.Now;
            listBox1.Items.Add("万条数据插入" + (dt2 - dt).TotalMilliseconds + "毫秒");

            dbc.updatetable("ddd", "id<10", new { name = "特大喜讯" });//有条件的修改，条件可以是空的
            int count = 0;
           
            dt = DateTime.Now;
          
            var rrs = dbc.selecttable<List<user>>("ddd", "id<100", 0, "", 0, 0, out count);//有条件的查询数据，条件可以是空的
          
            dt2 = DateTime.Now;
            listBox1.Items.Add("数据SQL查询" + (dt2 - dt).TotalMilliseconds + "毫秒。"+"查询数量:"+ rrs.Count);

            dbc.deletetable("ddd", "id<100");//有条件的删除数据，条件不可以是空的

            dbc.Removetable("ddd");
            dbc.close();
```
#### 运行测试
 
- K-V运行效率测试，万次读写1.8秒，配置I5 8G 128G 500G 台式机
![K-V运行效率测试，万次读写1.8秒，配置I5 8G 128G 500G 台式机](https://images.gitee.com/uploads/images/2018/1206/204019_b22fff09_598831.png "在这里输入图片标题")
- K-V运行效率测试，万次读写1.8秒，配置SURFACEBOOK 一代 8G 128G
![K-V运行效率测试，万次读写1.8秒，配置SURFACEBOOK 一代 8G 128G](https://images.gitee.com/uploads/images/2018/1201/092336_926426c6_598831.png "a0f86c0262df10cc9cc3c509714c935.png")

#### 参与贡献

1. Fork 本项目
2. 新建 Feat_xxx 分支
3. 提交代码
4. 新建 Pull Request


#### 码云特技

1. 使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2. 码云官方博客 [blog.gitee.com](https://blog.gitee.com)
3. 你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解码云上的优秀开源项目
4. [GVP](https://gitee.com/gvp) 全称是码云最有价值开源项目，是码云综合评定出的优秀开源项目
5. 码云官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6. 码云封面人物是一档用来展示码云会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)