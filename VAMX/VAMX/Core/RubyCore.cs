using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronRuby;
using IronRuby.Builtins;
using Microsoft.Scripting.Hosting;

namespace VAMX
{
    public static class RubyCore
    {
        private static ScriptEngine _engine;
        private static ScriptScope _scope;
        private static string self_path;

        public static ScriptEngine Engine { get { return _engine; } }
        public static ScriptScope Scope { get { return _scope; } }
        public static string SelfPath { get { return self_path; } set { self_path = value; } }

        public static void Initialize()
        {
            self_path = System.IO.Directory.GetCurrentDirectory();
            ScriptRuntime _runTime = Ruby.CreateRuntime();
            _engine = Ruby.GetEngine(_runTime);
            _scope = _engine.CreateScope();
            _engine.Execute(@"load_assembly 'IronRuby.Libraries'; load_assembly 'DataStructure'", _scope);
            _engine.ExecuteFile(@"rb\mapx.rb");
            _scope.SetVariable("a", "");
            _engine.Execute(@"$path = a", _scope);
        }

        public static MapX LoadMap(string path)
        {
            _scope.SetVariable("a", path);
            _engine.Execute(@"$map_path = a", _scope);
            _engine.ExecuteFile(@"rb\load_map.rb");
            var map = _engine.Execute<MapX>("$clr_map");
            _engine.Execute("$map = nil; $clr_map = nil; GC.start");
            return map;
        }

        public static void SaveMap(MapX map, string path)
        {
            _scope.SetVariable("a", path);
            _engine.Execute(@"$map_path = a", _scope);
            _scope.SetVariable("a", map);
            _engine.Execute(@"$clr_map = a", _scope);
            _engine.ExecuteFile(@"rb\save_map.rb");
            _engine.Execute("$map = nil; $clr_map = nil; GC.start");
        }

    }
}
