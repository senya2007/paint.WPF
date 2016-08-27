﻿using Paint.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Helpers
{
    public class ModelsCollection
    {
        private static ModelsCollection instance;
        public IEnumerable<IModel> AllModels;

        public ModelsCollection()
        {
           AllModels = GetEnumerableOfType<IModel>();
        }

        public static ModelsCollection GetInstance()
        {
            if (instance == null)
                instance = new ModelsCollection();
            return instance;
        }

        private static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }
    }
}
