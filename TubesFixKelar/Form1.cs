using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace TubesFixKelar
{
    class Node
    {
        public string name;
        public ArrayList connected;
        public Node(string n)
        {
            name = n;
            connected = new ArrayList();
        }
        public void addEdge(Node goal)
        {
            connected.Add(goal);
            goal.connected.Add(this);
        }
    }


    // Kelas dasar
    abstract class GraphSearcher
    {
        public virtual bool Bound(Node node, uint dist)
        {
            return false;
        }
        public virtual bool Goal(Node node)
        {
            return false;
        }
        public virtual void Traverse(Node node, Node parent)
        {
        }

        // Mencari di node
        public abstract Node search(Node start);
        //A B
        //A C
        //A D
        //B E
        //C F
    }

    struct QueueData
    {
        public Node node;
        public Node parent;
        public uint depth;
    }

    class BFSearch : GraphSearcher
    {
        public override Node search(Node start)
        {
            Queue q = new Queue();
            foreach (Node n in start.connected)
            {
                if (Goal(n))
                    return n;
                if (!Bound(n, 1))
                {
                    QueueData data;
                    data.node = n;
                    data.parent = start;
                    data.depth = 1;
                    q.Enqueue(data);
                }
            }
            while (q.Count > 0)
            {
                QueueData data = (QueueData)q.Dequeue();
                Traverse(data.node, data.parent);
                data.depth += 1;
                foreach (Node n in data.node.connected)
                {
                    if (Goal(n))
                        return n;
                    if (!Bound(n, 1))
                    {
                        QueueData temp;
                        temp.node = n;
                        temp.parent = data.node;
                        temp.depth = data.depth;
                        q.Enqueue(temp);
                    }
                }
            }
            return null;
        }
    }

    class DFSearch : GraphSearcher
    {
        public override Node search(Node start)
        {
            Stack q = new Stack();
            foreach (Node n in start.connected)
            {
                if (Goal(n))
                    return n;
                if (!Bound(n, 1))
                {
                    QueueData data;
                    data.node = n;
                    data.parent = start;
                    data.depth = 1;
                    q.Push(data);
                }
            }
            while (q.Count > 0)
            {
                QueueData data = (QueueData)q.Pop();
                Traverse(data.node, data.parent);
                data.depth += 1;
                foreach (Node n in data.node.connected)
                {
                    if (Goal(n))
                        return n;
                    if (!Bound(n, 1))
                    {
                        QueueData temp;
                        temp.node = n;
                        temp.parent = data.node;
                        temp.depth = data.depth;
                        q.Push(temp);
                    }
                }
            }
            return null;
        }
    }

    class MutualFriendDFS : DFSearch
    {
        private Node target;
        private ArrayList result;

        public override bool Bound(Node node, uint dist)
        {
            return (dist <= 2);
        }
        public override bool Goal(Node node)
        {
            return false;
        }
        public override void Traverse(Node node, Node parent)
        {
            if (node.name == target.name)
                result.Add(parent);
        }

        public static ArrayList start(Node s, Node t)
        {
            MutualFriendDFS obj = new MutualFriendDFS();
            obj.target = t;
            obj.result = new ArrayList();
            obj.search(s);
            return obj.result;
        }
    }

    class MutualFriendBFS : BFSearch
    {
        private Node target;
        private ArrayList result;

        public override bool Bound(Node node, uint dist)
        {
            return (dist <= 2);
        }
        public override bool Goal(Node node)
        {
            return false;
        }
        public override void Traverse(Node node, Node parent)
        {
            if (node.name == target.name)
                result.Add(parent);
        }

        public static ArrayList start(Node s, Node t)
        {
            MutualFriendBFS obj = new MutualFriendBFS();
            obj.target = t;
            obj.result = new ArrayList();
            obj.search(s);
            return obj.result;
        }
    }

    class PathDFS: DFSearch {
        private Node target;
        private Hashtable map;

        public override bool Bound(Node node, uint dist) {
            return false;
        }
        public override bool Goal(Node node) {
            return node.name == target.name;
        }
        public override void Traverse(Node node, Node parent) {
            map.Add(node.name, parent);
        }

        public static ArrayList start(Node s, Node t) {
            PathDFS obj = new PathDFS();
            obj.target = t;
            obj.map = new Hashtable();
            if (obj.search(s) == null)
                return null; // unreachable
            ArrayList result = new ArrayList();
            Node n = obj.target;
            while (Object.ReferenceEquals(n, s)) {
                result.Add(n);
                n = (Node)obj.map[n.name];
            }
            result.Add(s);
            return result;
        }
    }

    class PathBFS: BFSearch {
        private Node target;
        private Hashtable map;

        public override bool Bound(Node node, uint dist) {
            return false;
        }
        public override bool Goal(Node node) {
            return node.name == target.name;
        }
        public override void Traverse(Node node, Node parent) {
            map.Add(node.name, parent);
        }

        public static ArrayList start(Node s, Node t) {
            PathBFS obj = new PathBFS();
            obj.target = t;
            obj.map = new Hashtable();
            if (obj.search(s) == null)
                return null; // unreachable
            ArrayList result = new ArrayList();
            Node n = obj.target;
            while (Object.ReferenceEquals(n, s)) {
                result.Add(n);
                n = (Node)obj.map[n.name];
            }
            result.Add(s);
            return result;
        }
    }

    

    public partial class Form1 : Form
    {

        private Microsoft.Msagl.Drawing.Graph visualization;
        public Form1()  
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        ArrayList parseString(string str)
        {
            Hashtable nodes = new Hashtable();
            int length = str.Length;
            int i = 0;
            Hashtable map = new Hashtable();
            while (i < length)
            {
                int f = i;
                while (str[i] != ' ')
                {
                    i += 1;
                    if (i == length)
                        break;
                }
                if (i != length)
                    i -= 1;
                string name = str.Substring(f, i - f);
                i += 1;
                ArrayList edges = new ArrayList();
                while ((i < length) && (str[i] != '\n'))
                {
                    f = i;
                    while (str[i] != ' ')
                    {
                        i += 1;
                        if (i >= length)
                            break;
                    }
                    if (i != length)
                        i -= 1;
                    edges.Add(str.Substring(f, i - f));
                    i += 1;
                }
                map.Add(name, edges);
                nodes.Add(name, new Node(name));
            }
            foreach (string name in nodes.Keys)
            {
                Node node = (Node) nodes[name];
                ArrayList edges = (ArrayList)map[name];
                foreach (string edgename in edges)
                {
                    node.addEdge((Node)nodes[edgename]);
                }
            }
            ArrayList ret = new ArrayList();
            foreach (Node node in nodes.Values)
            {
                ret.Add(node);
            }
            return ret;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {   
                   
                    string text = File.ReadAllText(file);
                    size = text.Length;
                    ArrayList Node = parseString(text);
                    this.comboBox2.Enabled = true;
                    this.label8.Text = Path.GetFileName(file);
                    this.label8.Visible = true;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
            this.comboBox1.Enabled = false;
            this.comboBox2.Enabled = false;
            this.button2.Enabled = false;
            this.label8.Visible = false;
            this.checkBox1.Enabled = false;

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        { 
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.checkBox1.Enabled = true;
            this.button2.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ((this.checkBox1.Checked) == true)
            {
                this.comboBox1.Enabled = true;
                this.radioButton1.Enabled = true;
                this.radioButton2.Enabled = true;
                if ((this.comboBox1.SelectedItem == null) && (this.radioButton1.Checked == true || this.radioButton2.Checked == true))
                {
                    this.button2.Enabled = true;
                } else
                {
                    this.button2.Enabled = false;
                }
            } else
            {
                this.comboBox1.ResetText();
                this.comboBox1.Enabled = false;
                this.radioButton1.Checked = false;
                this.radioButton1.Enabled = false;
                this.radioButton2.Checked = false;
                this.radioButton2.Enabled = false;
            }
        }
    }
}
