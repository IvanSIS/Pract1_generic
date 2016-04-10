using System;
using System.Collections.Generic;
using System.Linq;


namespace proc_repo
{

    public interface IProc<TEngine>
    {
        IProc<TEngine, TEntity> For<TEntity>();
    }

    public class Processor<TEngine> : IProc<TEngine>
    {
        public IProc<TEngine, TEntity> For<TEntity>()
        {
            return new Processor<TEngine, TEntity>();
        }
    }


    public interface IProc<TEngine, TEntity>
    {
        Processor<TEngine, TEntity, TLogger> With<TLogger>();
    }

    public class Processor<TEngine, TEntity> : IProc<TEngine, TEntity>
    {
        public Processor<TEngine, TEntity, TLogger> With<TLogger>()
        {
            return new Processor<TEngine, TEntity, TLogger>();
        }
    }

    public interface IProc
    {
        IProc<TEngine> Make<TEngine>();
    }

    
    public class Processor : IProc
    {
        public IProc<TEngine> Make<TEngine>()
        {
            return new Processor<TEngine>();
        }

        public static IProc<TEngine> CreateEngine<TEngine>()
        {
            return new Processor().Make<TEngine>();
        }
    }


    public class Processor<TEngine, TEntity, TLogger> { }

    //repository

    public class GuidRepo
    {
        private Dictionary<Guid, object> Repo;

        public GuidRepo()
        {
            Repo = new Dictionary<Guid, object>();
        }

        public K make<K>() where K : new()
        {
            var guid = Guid.NewGuid();
            return (K) (Repo[guid] = new K());
        }

        public Dictionary<Guid, K> TakeDict<K>()
        {
             var R = Repo
                .Where(pair => pair.Value is K)
                .ToDictionary(pair => pair.Key, pair => (K)pair.Value);

            return R;
        }

        public K Take<K>(Guid guid)
        {
            if (!Repo.ContainsKey(guid))
                return default(K);
            var entity = Repo[guid];
            if (entity is K)
                return (K)entity;
            return default(K);
        }
    }
}


namespace lab1
{
    using proc_repo;    

    class MyEngine
    {

    }

    class MyEntity
    {

    }

    class MyLogger
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            var p = Processor.CreateEngine<MyEngine>().For<MyEntity>().With<MyLogger>();
            Console.WriteLine(p.GetType());

            var repo = new GuidRepo();
            repo.make<MyEntity>();
            repo.make<MyLogger>();
            repo.make<MyEngine>();
            Console.WriteLine(repo.TakeDict<MyEntity>().Count);
            Console.WriteLine(repo.TakeDict<MyLogger>().Count);
            Console.WriteLine(repo.TakeDict<MyEngine>().Count);

            Console.WriteLine(repo.Take<MyEntity>(repo.TakeDict<MyEntity>().First().Key));
            Console.WriteLine(repo.Take<MyLogger>(repo.TakeDict<MyLogger>().First().Key));
            Console.WriteLine(repo.Take<MyEngine>(repo.TakeDict<MyEngine>().First().Key));


        }
    }
}
