> 写在项目介绍前面，有些人觉得自己写一个DB内容，浪费时间，做的不会比成熟的好，有些人觉得C#效率一定很低，我的一些好朋友也觉的我写这个项目是存粹造轮子，我想说的是：1.之前开源了相关socket的架构，为了证明socket架构可以适应绝大部分使用场景，自己写了很多基于架构通信的项目做个应用示例。2.DB是个对于效率速度要求很高的东西，强迫我想尽办法提升效率。从最初SQL语句1000W数据条件查询90秒，到优化后150毫秒内。从KV查询40毫秒到0毫秒。想尽了各种办法。不但使我对一些内容有新的领悟，也使得DB项目和socket通信架构的效率，都提升几十倍。并发现了之前socket架构中的隐藏很深的多线程BUG。我觉得这一切都是值得的。3.总有些机构的半吊子培训，告诉大家.net这不行那不行，这不能做那不能做。做了12年.net开发的我真心不舒服，我使用c#开发其实只有2个理由:我只会用VS开发并且C#语法熟悉。

### # WeavingDB


#### 项目介绍
为满足C#项目的特殊使用与简单部署，而开发此WeavingDB。


- 1.支持K-V方式。
- 2.支持JSON数据的条件查询的内存缓存库。（目前JSON库部分，暂时未开放，持续更新）
- 3.可以设置最后一次访问时间的过期时效，例如：A数据，最后一次访问是1小时以前，设置过期为60分钟，此时A数据将被清理，如果A数据在60分钟内再次被访问则不清理。也可以设置永久不过期。
- 数据通信部分使用WeavingSocket架构，通信部分架构地址：https://gitee.com/dreamsfly900/universal-Data-Communication-System-for-windows

![K-V运行效率测试，万次读写1.8秒，配置SURFACEBOOK 一代 8G 128G](https://images.gitee.com/uploads/images/2018/1201/092336_926426c6_598831.png "a0f86c0262df10cc9cc3c509714c935.png")
#### 软件架构
软件架构说明


#### 安装教程

1. xxxx
2. xxxx
3. xxxx

#### 使用说明

 `  DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
        double gggg = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            gggg = 0;
            dbc.open();
            dbc.Set<String>("asdasd", "1");
            int i = 0;
            String str2 = dbc.Get<String>("asdasd");
            while (i < 10000)
            {
                i++;
                DateTime dt = DateTime.Now;
                String str = dbc.Get<String>("asdasd");
                DateTime dt2 = DateTime.Now;
                double gg= (dt2 - dt).TotalMilliseconds;
                gggg += gg;
                listBox1.Items.Add("第" + (i++) + "次查询耗时：" + gg + "毫秒");
            }
        
            label1.Text = "1W次总耗时："+ gggg;
            dbc.close();
        }`

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