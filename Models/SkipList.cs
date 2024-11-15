namespace Luo.Web.Host.Models;

/// <summary>
/// SkipList Node class (跳表节点)
/// </summary>
/// <param name="level"></param>
/// <param name="customer"></param>
public class Node(int level, Customer customer)
{
    public Customer Value = customer;
    public Node[] Forward = new Node[level + 1];
}

/// <summary>
/// SkipList class (跳表）
/// </summary>
public class SkipList
{
    private readonly int maxLevel;
    private static readonly double probability = 0.5;

    private Node head;
    private int currentLevel;

    /// <summary>
    /// contructor
    /// </summary>
    /// <param name="level">the max number of index level</param>
    public SkipList(int level)
    {
        this.maxLevel = level;
        this.head = new Node(level, null);
    }

    public int RandomLevel()
    {
        int level = 0;
        Random random = new();
        while (random.NextDouble() < probability && level < maxLevel)
        {
            level++;
        }
        return level;
    }

    /// <summary>
    /// Add a customer.
    /// Note: the added customer already sorted by Score desc, by CustomerID asc
    /// </summary>
    /// <param name="customer"></param>
    public void Insert(Customer customer)
    {
        Node[] update = new Node[maxLevel + 1];
        Node current = head;

        // 找到要插入位置
        for (int i = currentLevel; i >= 0; i--)
        {
            while (current.Forward[i] != null &&
                (current.Forward[i].Value.Score > customer.Score ||
                (current.Forward[i].Value.Score == customer.Score && current.Forward[i].Value.CustomerId < customer.CustomerId)))
            {
                current = current.Forward[i];
            }
            update[i] = current;
        }

        current = current.Forward[0];

        // 如果值已存在，则不插入
        if (current != null && current.Value.CustomerId == customer.CustomerId)
        {
            return;
        }

        // 生成随机层级并插入节点
        int level = RandomLevel();
        if (level > currentLevel)
        {
            for (int i = currentLevel + 1; i <= level; i++)
            {
                update[i] = head;
            }
            currentLevel = level;
        }

        Node newNode = new(level, customer);
        for (int i = 0; i <= level; i++)
        {
            newNode.Forward[i] = update[i].Forward[i];
            update[i].Forward[i] = newNode;
        }
    }

    /// <summary>
    /// delete the customer.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public bool Delete(long customerId)
    {
        Node[] update = new Node[maxLevel + 1];
        Node current = head;

        for (int i = currentLevel; i >= 0; i--)
        {
            while (current.Forward[i] != null && current.Forward[i].Value.CustomerId < customerId)
            {
                current = current.Forward[i];
            }
            update[i] = current;
        }

        current = current.Forward[0];

        // 如果找到了值，则删除
        if (current != null && current.Value.CustomerId == customerId)
        {
            for (int i = 0; i <= currentLevel; i++)
            {
                if (update[i].Forward[i] != current)
                {
                    break;
                }
                update[i].Forward[i] = current.Forward[i];
            }

            // 调整当前层级
            while (currentLevel > 0 && head.Forward[currentLevel] == null)
            {
                currentLevel--;
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Update the customer's score.
    /// (1) delete old customer;
    /// (2) add a new customer whith the updated score (only when the new score is greater than 0)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="score"></param>
    /// <returns>The updated score if the customer found, otherwise return 0.</returns>
    public decimal UpdateScore(long customerId, decimal score)
    {
        Node current = head;
        //TODO: the find logic need to check again.
        for (int i = currentLevel; i >= 0; i--)
        {
            while (current.Forward[i] != null && current.Forward[i].Value.CustomerId < customerId)
            {
                current = current.Forward[i];
            }
        }

        // if customer found，get the score， then delete it and add a new customer with new score.
        current = current.Forward[0];
        if (current != null && current.Value.CustomerId == customerId)
        {
            decimal oldScore = current.Value.Score;
            Delete(customerId);
            decimal newScore = oldScore + score;

            if (newScore > 0)
            {
                Insert(new Customer() { CustomerId = customerId, Score = newScore });
            }

            Console.WriteLine(string.Format("CustomerID: {0}---Old Score: {1} ---Update Score: {2} --- NewScore: {3}", customerId, oldScore, score, newScore));
            return newScore;
        }

        return 0;
    }

    /// <summary>
    /// Get nodes by the ranking region.
    /// </summary>
    /// <param name="start">start ranking</param>
    /// <param name="end">end randing</param>
    /// <returns>the found nodes  which the ranking is between start and end.</returns>
    public List<LeaderBoard> GetNodesByRankingRegion(int start, int end)
    {
        //收集所有客户到列表中
        var list = new List<LeaderBoard>();

        int rank = 1;
        Node current = head.Forward[0];
        while (current != null)
        {
            if (rank >= start && rank <= end)
            {
                list.Add(new LeaderBoard { CustomerId = current.Value.CustomerId, Score = current.Value.Score, Rank = rank });
            }
            rank++;
            current = current.Forward[0];
            if(rank > end)
            {
                break;
            }
        }

        return list;
    }

    /// <summary>
    /// Get the ranking
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns>the found customer's ranking according it's score, otherwise return 0; </returns>
    public int GetRanking(long customerId)
    {
        int rank = 0;
        Node current = head.Forward[0];
        while (current != null)
        {
            rank++;
            if (current.Value.CustomerId == customerId)
            {
                break;
            }
            current = current.Forward[0];
        }

        return rank;
    }

    public void PrintList()
    {
        Console.WriteLine("---------Skip List Data------------------------------");
        for (int i = 0; i <= currentLevel; i++)
        {
            Console.WriteLine(string.Format("---------------Level {0}---------------------------------------------------", i));
            Node node = head.Forward[i];

            int nodeCount = 0;
            while (node != null)
            {
                //Console.WriteLine(string.Format("CustomerID: {0} ---Score: {1}", node.Value?.CustomerId, node.Value?.Score));
                node = node.Forward[i];
                nodeCount++;
            }
            Console.WriteLine(string.Format("---------Level {0}, Node Count: {1}-----------------------------------------", i, nodeCount));
            Console.WriteLine();
        }
    }
}
