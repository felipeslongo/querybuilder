using System;

namespace SqlKata.Compilers
{
    public class MySqlCompiler : Compiler
    {
        public MySqlCompiler() : base()
        {
            EngineCode = "mysql";
            OpeningIdentifier = ClosingIdentifier = "`";
        }

        public override string CompileOffset(SqlResult ctx)
        {
            var limitOffset = ctx.Query.GetOneComponent("limit", EngineCode) as LimitOffset;

            if (limitOffset == null || !limitOffset.HasOffset())
            {
                return "";
            }

            ctx.Bindings.Add(limitOffset.Offset);

            // MySql will not accept offset without limit
            // So we will put a large number to avoid this error
            if (!limitOffset.HasLimit())
            {
                return "LIMIT 18446744073709551615 OFFSET ?";
            }

            return "OFFSET ?";
        }
    }

    public static class MySqlCompilerExtensions
    {
        public static string ENGINE_CODE = "mysql";
        public static Query ForMySql(this Query src, Func<Query, Query> fn)
        {
            return src.For(MySqlCompilerExtensions.ENGINE_CODE, fn);
        }
    }
}