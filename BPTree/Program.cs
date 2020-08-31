using SQLDBlogic.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WeavingDB.Logical;

namespace BPTree
{
    class Program
    {
        static void Main(string[] args)
        {
            BPTree bpt = new BPTree(3);
            //bpt.insert(bpt.root, 1);
            //bpt.insert(bpt.root, 2);
            //bpt.insert(bpt.root, 3);
            //bpt.insert(bpt.root, 4);
            //bpt.insert(bpt.root, 3);
            //bpt.insert(bpt.root, 4);
            //bpt.insert(bpt.root, 5);
            //bpt.insert(bpt.root, 6);
            //bpt.root.printBPlusTree(0);
            //bpt.remove(3);
            //bpt.root.printBPlusTree(0);
            //Node nd= bpt.search(bpt.root, 3);

        }
    }
    /**
 * B+树的定义：
 * 1.任意非叶子结点最多有M个子节点；且M>2；M为B+树的阶数
 * 2.除根结点以外的非叶子结点至少有 (M+1)/2个子节点；
 * 3.根结点至少有2个子节点；
 * 4.除根节点外每个结点存放至少（M-1）/2和至多M-1个关键字；（至少1个关键字）
 * 5.非叶子结点的子树指针比关键字多1个；
 * 6.非叶子节点的所有key按升序存放，假设节点的关键字分别为K[0], K[1] … K[M-2],指向子女的指针分别为P[0], P[1]…P[M-1]。则有：
 * 　　　P[0] < K[0] <= P[1] < K[1] …..< K[M-2] <= P[M-1]
 * 7.所有叶子结点位于同一层；
 * 8.为所有叶子结点增加一个链指针；
 * 9.所有关键字都在叶子结点出现
 */
    public unsafe class BPTree
    {
      public  int order = 0;
        public int height = 0;
        public Node root;
        public Node head;
        public BPTree(int _order)
        {
            order = _order;
            root = new Node(true,_order);
        }
        byte datatype = 0;
        /// <summary>
        /// 比较逻辑
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="p1">数据1</param>
        /// <param name="mtsContrast">比较类型0>=，1<=,2==,3>,4<,5like</param>
        /// <param name="valuep1">数据2</param>
        /// <returns></returns>
        public void insert(Node root,void* key,IntPtr value,byte _datatype)
        {
            datatype = _datatype;
            root.datatype = _datatype;
            root.inset(key, value, this); 
        }
        public Node search(Node root, void* key)
        {
            return root.get(key);
        }
        public void remove(void* key)
        {
              root.remove(key, this);
        }
    }
    public unsafe class KV{
        public void* key;
   
        public List<IntPtr> value=new List<IntPtr>();
    }
    public unsafe class    Node
    {
        public Node()
        {
             
        }
        public byte datatype = 0;
        public   int order = 0;
        public Node(bool f,   int _order = 0)
        {
            order = _order;
            isLeaf = f;
        }
        // 是否为叶子节点
        public bool isLeaf;
        public int count = 0;
        public List<KV> keys=new List<KV>();
        public Node previous;
        public Node parent;
        // 叶节点的后节点
        public Node next;
        public List<Node> Child=new List<Node>();
        private bool isRoot;
        public Node get(void* key)
        {
            //如果是叶子节点 
            if (isLeaf)
            {
                int low = 0, high = keys.Count - 1, mid;
                KV comp;
                
                while (low <= high)
                {
                    mid = (low + high) / 2;
                    comp = keys[mid];
                    if (utli.CompareLogical(datatype, comp.key,2, key) )
                    {
                        return this;
                    }
                    else if (utli.CompareLogical(datatype, comp.key, 4, key))
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                //未找到所要查询的对象 
                return null;
            }
            //如果不是叶子节点 
            //如果key小于节点最左边的key，沿第一个子节点继续搜索 
            if (utli.CompareLogical(datatype, key, 4, keys[0].key) )
            {
                return Child[0].get(key);
            }
             if (utli.CompareLogical(datatype, key, 2, keys[0].key)  )
            {
                int i = 0;
               
                while( i< Child.Count && Child[i].get(key) == null)
                    i++;
                if (i < Child.Count)
                    return Child[i].get(key);
                else
                    return null;
                //如果key大于等于节点最右边的key，沿最后一个子节点继续搜索 
            }
            else if (utli.CompareLogical(datatype, key, 0, keys[keys.Count - 1].key))
            {
                return Child[Child.Count- 1].get(key);
                //否则沿比key大的前一个子节点继续搜索 
            }
            else
            {
                int low = 0, high = keys.Count - 1, mid = 0;
                KV comp;
                while (low <= high)
                {
                    mid = (low + high) / 2;
                    comp = keys[mid];
                    if (utli.CompareLogical(datatype, comp.key, 2, key) )
                    {
                        return Child[mid + 1].get(key);
                    }
                    else if (utli.CompareLogical(datatype, comp.key, 4, key)  )
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                return Child[low] .get(key);
            }
        }
        protected void insertOrUpdate(void* key, IntPtr value)
        {
            //二叉查找，插入
            int low = 0, high = keys.Count  - 1, mid;
            KV comp;
            while (low <= high)
            {
                mid = (low + high) / 2;
                comp = keys[mid];
                if (utli.CompareLogical(datatype, comp.key,2, key))
                {
                   // keys[mid].setValue(value);
                    break;
                }
                else if (utli.CompareLogical(datatype, comp.key, 4, key))
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            if (low > high)
            {
                KV KK = new KV();
                KK.key = key;
                KK.value.Add(value);
                keys.Insert(low, KK);
            }
        }
        public void inset(void* key, IntPtr value, BPTree bPTree)
        {
            if (isLeaf)
            {
                if (contains(key) != -1 || keys.Count < order)
                {

                    //   keys.Add(key);
                    insertOrUpdate(key,value);
                    if (bPTree.height == 0)
                    {
                        bPTree.height++;
                    }
                    return;
                }
                Node left = new Node(true, order);
                Node right = new Node(true, order);
                if (previous != null)
                {
                    previous.next = left;
                    left.previous = previous;
                }
                if (next != null)
                {
                    next.previous = right;
                    right.next = next;
                }
                if (previous == null)
                {
                    bPTree.head=(left);
                }
                left.next = right;
                right.previous = left;
                previous = null;
                next = null;

                copy2Nodes(key, left, right, bPTree);
                if (parent != null)
                {
                    //调整父子节点关系 
                    int index = parent.Child.IndexOf(this);
                    parent.Child.Remove(this);
                    left.parent = parent;
                    right.parent = parent;
                    parent.Child.Insert(index, left);
                    parent.Child.Insert(index + 1, right);
                    parent.keys.Insert(index, right.keys[0]);
                    keys = null; //删除当前节点的关键字信息
                    Child = null; //删除当前节点的孩子节点引用

                    //父节点插入或更新关键字 
                    parent.updateInsert(bPTree);
                    parent = null; //删除当前节点的父节点引用
                                   //如果是根节点     
                }
                else
                {
                    isRoot = false;
                    Node parent = new Node(false, order);
                    parent.isRoot = true;
                    bPTree.root=(parent);
                    left.parent = parent;
                    right.parent = parent;
                    parent.Child.Add(left);
                    parent.Child.Add(right);
                    parent.keys.Add(right.keys[0]);
                    keys = null;
                    Child = null;
                }
                return;
            }
            //比较类型0>=，1<=,2==,3>,4<,5like
            //如果不是叶子节点
            //如果key小于等于节点最左边的key，沿第一个子节点继续搜索 
            if (utli.CompareLogical(datatype, key,4, keys[0].key) )
            {
                Child[0].inset(key,value, bPTree);
                //如果key大于节点最右边的key，沿最后一个子节点继续搜索 
            }
            else if (utli.CompareLogical(datatype,key ,0,keys[keys.Count - 1].key))
            {
                Child[Child.Count - 1].inset(key,value, bPTree);
                //否则沿比key大的前一个子节点继续搜索 
            }
            else
            {
                int low = 0, high = keys.Count - 1, mid = 0;
                KV comp;
                while (low <= high)
                {
                    mid = (low + high) / 2;
                    comp = keys[mid];
                    if (utli.CompareLogical(datatype, comp.key,2, key))
                    {
                        Child[mid + 1].inset(key,value, bPTree);
                        break;
                    }
                    else if (utli.CompareLogical(datatype, comp.key, 4, key))
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                if (low > high)
                {
                    Child[(low)].inset(key,value, bPTree);
                }
            }

        }

        protected void updateInsert(BPTree tree)
        {

            //如果子节点数超出阶数，则需要分裂该节点    
            if (Child.Count  > tree.order)
            {
                //分裂成左右两个节点 
                Node left = new Node(false,order);
                Node right = new Node(false, order);
                //左右两个节点子节点的长度 
                int leftSize = (tree.order + 1) / 2 + (tree.order + 1) % 2;
                int rightSize = (tree.order + 1) / 2;
                //复制子节点到分裂出来的新节点，并更新关键字 
                for (int i = 0; i < leftSize; i++)
                {
                    left.Child.Add(Child[i]);
                    Child[i].parent = left;
                }
                for (int i = 0; i < rightSize; i++)
                {
                    right.Child.Add(Child[leftSize + i]);
                    Child[leftSize + i].parent = right;
                }
                for (int i = 0; i < leftSize - 1; i++)
                {
                    left.keys.Add(keys[i]);
                }
                for (int i = 0; i < rightSize - 1; i++)
                {
                    right.keys.Add(keys[leftSize + i]);
                }

                //如果不是根节点 
                if (parent != null)
                {
                    //调整父子节点关系 
                    int index = parent.Child.IndexOf(this);
                    parent.Child.Remove(this);
                    left.parent = parent;
                    right.parent = parent;
                    parent.Child.Insert(index, left);
                    parent.Child.Insert(index + 1, right);
                    parent.keys.Insert(index, keys[leftSize - 1]);
                    keys = null;
                    Child = null;

                    //父节点更新关键字 
                    parent.updateInsert(tree);
                    parent = null;
                    //如果是根节点     
                }
                else
                {
                    isRoot = false;
                    Node parent = new Node(false,order);
                    parent.isRoot = true;
                    tree.root=(parent);
                    tree.height=tree.height + 1;
                    left.parent = parent;
                    right.parent = parent;
                    parent.Child.Add(left);
                    parent.Child.Add(right);
                    parent.keys.Add(keys[leftSize - 1]);
                    keys = null;
                    Child = null;
                }
            }
        }
        private void copy2Nodes(void* key, Node left,
                           Node right, BPTree tree)
        {
            //左右两个节点关键字长度 
            int leftSize = (tree.order  + 1) / 2 + (tree.order + 1) % 2;
            bool b = false;//用于记录新元素是否已经被插入
            for (int i = 0; i < keys.Count; i++)
            {
                if (leftSize != 0)
                {
                    leftSize--;
                    if (!b && utli.CompareLogical(datatype, keys[i].key,3, key)   )
                    {
                        KV kk = new KV();
                        kk.key = key;
                        left.keys.Add(kk);
                        b = true;
                        i--;
                    }
                    else
                    {
                        left.keys.Add(keys[i]);
                    }
                }
                else
                {
                    if (!b && utli.CompareLogical(datatype, keys[i].key, 3, key)  )
                    {
                        KV kk = new KV();
                        kk.key = key;
                        right.keys.Add(kk);
                        b = true;
                        i--;
                    }
                    else
                    {
                        right.keys.Add(keys[i]);
                    }
                }
            }
            if (!b)
            {
                KV kk = new KV();
                kk.key = key;
                right.keys.Add(kk);
            }
        }
        public int contains(void* key)
        {
            int low = 0, high = keys.Count - 1, mid;
            KV comp;
            while (low <= high)
            {
                mid = (low + high) / 2;
                comp = keys[mid];
                if (utli.CompareLogical(datatype, comp.key, 2, key)  )
                {
                    return mid;
                }
                else if (utli.CompareLogical(datatype, comp.key, 4, key) )
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return -1;
        }


        public void printBPlusTree(int index)
        {
            if (this.isLeaf)
            {
              Console.Write("层级：" + index + ",叶子节点，keys为: ");
                for (int i = 0; i < keys.Count; ++i)
                    Console.Write(keys[i] + " ");
                Console.WriteLine();
            }
            else
            {
                Console.Write("层级：" + index + ",非叶子节点，keys为: ");
                for (int i = 0; i < keys.Count; ++i)
                    Console.Write(keys[i] + " ");
                Console.WriteLine();
                for (int i = 0; i < Child.Count; ++i)
                    Child[i].printBPlusTree(index + 1);
            }
        }

        public void remove(void* key, BPTree tree)
        {
            //如果是叶子节点 
            if (isLeaf)
            {
                //如果不包含该关键字，则直接返回 
                if (contains(key) == -1)
                {
                    return ;
                }
                //如果既是叶子节点又是根节点，直接删除 
                if (isRoot)
                {
                    if (keys.Count == 1)

                    {
                        tree.height=(0);
                    }
                     remove(key);
                    return;
                }
                //如果关键字数大于M / 2，直接删除 
                if (keys.Count > tree.order / 2 && keys.Count > 2)
                {
                     remove(key);
                    return;
                }
                //如果自身关键字数小于M / 2，并且前节点关键字数大于M / 2，则从其处借补 
                if (previous != null &&
                        previous.parent == parent
                        && previous.keys.Count > tree.order / 2
                        && previous.keys.Count > 2)
                {
                    //添加到首位 
                    int size = previous.keys.Count;
                    keys.Insert(0, previous.keys[size - 1]);
                    previous.keys.RemoveAt(size - 1);
                    int index = parent.Child.IndexOf(previous);
                    parent.keys[index]= keys[0];
                     remove(key);
                    return;
                }
                //如果自身关键字数小于M / 2，并且后节点关键字数大于M / 2，则从其处借补 
                if (next != null
                        && next.parent == parent
                        && next.keys.Count > tree.order / 2
                        && next.keys.Count > 2)
                {
                    keys.Add(next.keys[0]);
                    next.keys.RemoveAt(0);
                    int index = parent.Child.IndexOf(this);
                    parent.keys[index]=next.keys[0];
                     remove(key);
                    return;
                }

                //同前面节点合并 
                if (previous != null
                        && previous.parent == parent
                        && (previous.keys.Count <= tree.order / 2
                        || previous.keys.Count <= 2))
                {
                     remove(key);
                    for (int i = 0; i < keys.Count; i++)
                    {
                        //将当前节点的关键字添加到前节点的末尾
                        previous.keys.Add(keys[i]);
                    }
                    keys = previous.keys;
                    parent.Child.Remove(previous);
                    previous.parent = null;
                    previous.keys = null;
                    //更新链表 
                    if (previous.previous != null)
                    {
                        Node temp = previous;
                        temp.previous.next = this;
                        previous = temp.previous;
                        temp.previous = null;
                        temp.next = null;
                    }
                    else
                    {
                        tree.head=(this);
                        previous.next = null;
                        previous = null;
                    }
                    parent.keys.RemoveAt(parent.Child.IndexOf(this));
                    if ((!parent.isRoot && (parent.keys.Count >= tree.order / 2
                            && parent.keys.Count >= 2))
                            || parent.isRoot && parent.Child.Count >= 2)
                    {
                        return ;
                    }
                    parent.updateRemove(tree);
                    return ;
                }
                //同后面节点合并
                if (next != null
                        && next.parent == parent
                        && (next.keys.Count <= tree.order/ 2
                        || next.keys.Count <= 2))
                {
                    remove(key);
                    for (int i = 0; i < next.keys.Count; i++)
                    {
                        //从首位开始添加到末尾 
                        keys.Add(next.keys[i]);
                    }
                    next.parent = null;
                    next.keys = null;
                    parent.Child.Remove(next);
                    //更新链表 
                    if (next.next != null)
                    {
                        Node temp = next;
                        temp.next.previous = this;
                        next = temp.next;
                        temp.previous = null;
                        temp.next = null;
                    }
                    else
                    {
                        next.previous = null;
                        next = null;
                    }
                    //更新父节点的关键字列表
                    parent.keys.RemoveAt(parent.Child.IndexOf(this));
                    if ((!parent.isRoot && (parent.Child.Count >= tree.order / 2
                            && parent.Child.Count >= 2))
                            || parent.isRoot && parent.Child.Count >= 2)
                    {
                        return ;
                    }
                    parent.updateRemove(tree);
                    return ;
                }
            }
            /*如果不是叶子节点*/

            //如果key小于等于节点最左边的key，沿第一个子节点继续搜索 
            if (utli.CompareLogical(datatype, key, 4, keys[0].key)  )
            {
                 Child[0].remove(key, tree);
                return;
                //如果key大于节点最右边的key，沿最后一个子节点继续搜索 
            }
            else if (utli.CompareLogical(datatype, key, 0, keys[keys.Count - 1].key) )
            {
                Child[Child.Count - 1].remove(key, tree);
                return;
                //否则沿比key大的前一个子节点继续搜索 
            }
            else
            {
                int low = 0, high = keys.Count- 1, mid = 0;
                KV comp;
                while (low <= high)
                {
                    mid = (low + high) / 2;
                    comp = keys[mid];
                    if (utli.CompareLogical(datatype, comp.key, 2, key) )
                    {
                         Child[mid + 1].remove(key, tree);
                        return;
                    }
                    else if (utli.CompareLogical(datatype, comp.key, 4, key))
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                 Child[low].remove(key, tree);
                return;
            }
        }
        protected void updateRemove(BPTree tree)
        {

            // 如果子节点数小于M / 2或者小于2，则需要合并节点 
            if (Child.Count < tree.order / 2 || Child.Count < 2)
            {
                if (isRoot)
                {
                    // 如果是根节点并且子节点数大于等于2，OK 
                    if (Child.Count >= 2) return;
                    // 否则与子节点合并 
                    Node root = Child[0];
                    tree.root=(root);
                    tree.height=(tree.height - 1);
                    root.parent = null;
                    root.isRoot = true;
                    keys = null;
                    Child = null;
                    return;
                }
                //计算前后节点  
                int currIdx = parent.Child.IndexOf(this);
                int prevIdx = currIdx - 1;
                int nextIdx = currIdx + 1;
                Node previous = null, next = null;
                if (prevIdx >= 0)
                {
                    previous = parent.Child[prevIdx];
                }
                if (nextIdx < parent.Child.Count)
                {
                    next = parent.Child[nextIdx];
                }

                // 如果前节点子节点数大于M / 2并且大于2，则从其处借补 
                if (previous != null
                        && previous.Child.Count > tree.order / 2
                        && previous.Child.Count > 2)
                {
                    //前叶子节点末尾节点添加到首位 
                    int idx = previous.Child.Count - 1;
                   Node borrow = previous.Child[idx];
                    previous.Child.RemoveAt(idx);
                    borrow.parent = this;
                    Child.Insert(0, borrow);
                    int preIndex = parent.Child.IndexOf(previous);

                    keys.Insert(0, parent.keys[preIndex]);
                    parent.keys[preIndex]= previous.keys[idx - 1];
                    previous.keys.RemoveAt(idx - 1);
                    return;
                }

                // 如果后节点子节点数大于M / 2并且大于2，则从其处借补
                if (next != null
                        && next.Child.Count > tree.order / 2
                        && next.Child.Count > 2)
                {
                    //后叶子节点首位添加到末尾 
                   Node borrow = next.Child[0];
                    next.Child.RemoveAt(0);
                    borrow.parent = this;
                    Child.Add(borrow);
                    int preIndex = parent.Child.IndexOf(this);
                    keys.Add(parent.keys[preIndex]);
                    parent.keys[preIndex]= next.keys[0];
                    next.keys.RemoveAt(0);
                    return;
                }

                // 同前面节点合并 
                if (previous != null
                        && (previous.Child.Count <= tree.order / 2
                        || previous.Child.Count <= 2))
                {
                    for (int i = 0; i <Child.Count; i++)
                    {
                        previous.Child.Add(Child[i]);
                    }
                    for (int i = 0; i < previous.Child.Count; i++)
                    {
                        previous.Child[i].parent = this;
                    }
                    int indexPre = parent.Child.IndexOf(previous);
                    previous.Child.Add(parent.Child[indexPre]);
                    for (int i = 0; i < keys.Count; i++)
                    {
                        previous.keys.Add(keys[i]);
                    }
                    Child = previous.Child;
                    keys = previous.keys;

                    //更新父节点的关键字列表
                    parent.Child.Remove(previous);
                    previous.parent = null;
                    previous.Child = null;
                    previous.keys = null;
                    parent.keys.RemoveAt(parent.Child.IndexOf(this));
                    if ((!parent.isRoot
                            && (parent.Child.Count >= tree.order / 2
                            && parent.Child.Count >= 2))
                            || parent.isRoot && parent.Child.Count >= 2)
                    {
                        return;
                    }
                    parent.updateRemove(tree);
                    return;
                }

                // 同后面节点合并 
                if (next != null
                        && (next.Child.Count <= tree.order / 2
                        || next.Child.Count <= 2))
                {
                    for (int i = 0; i < next.Child.Count; i++)
                    {
                        Node child = next.Child[i];
                        Child.Add(child);
                        child.parent = this;
                    }
                    int index = parent.Child.IndexOf(this);
                    keys.Add(parent.keys[index]);
                    for (int i = 0; i < next.keys.Count; i++)
                    {
                        keys.Add(next.keys[i]);
                    }
                    parent.Child.Remove(next);
                    next.parent = null;
                    next.Child = null;
                    next.keys = null;
                    parent.keys.RemoveAt(parent.Child.IndexOf(this));
                    if ((!parent.isRoot && (parent.Child.Count >= tree.order / 2
                            && parent.Child.Count >= 2))
                            || parent.isRoot && parent.Child.Count >= 2)
                    {
                        return;
                    }
                    parent.updateRemove(tree);
                    return;
                }
            }
        }
        protected void remove(void* key)
        {
            int low = 0, high = keys.Count - 1, mid;
            KV comp;
            while (low <= high)
            {
                mid = (low + high) / 2;
                comp = keys[mid];
                if (utli.CompareLogical(datatype, comp.key,2, key))
                {
                     keys.RemoveAt(mid);
                    return;
                }
                else if (utli.CompareLogical(datatype, comp.key, 4, key) )
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return ;
        }

       
    }
}
