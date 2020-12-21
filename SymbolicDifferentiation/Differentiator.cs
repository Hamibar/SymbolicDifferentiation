using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace SymbolicDifferentiation
{
    public static class Differentiator
    {
        private static Dictionary<ExpressionType, Func<Expression, Expression>> supportedFuncs;
        private static void InitializeSupportedFuncs()
        {
            supportedFuncs = new Dictionary<ExpressionType, Func<Expression, Expression>>
            {
                [ExpressionType.Multiply] = (expr) =>
                {
                    var e = (BinaryExpression)expr;
                    return Expression.Add(
                        Expression.Multiply(e.Left, supportedFuncs[e.Right.NodeType](e.Right)),
                        Expression.Multiply(e.Right, supportedFuncs[e.Left.NodeType](e.Left)));
                },

                [ExpressionType.Divide] = (expr) =>
                {
                    var e = (BinaryExpression)expr;
                    return Expression.Divide(
                        Expression.Subtract(
                            Expression.Multiply(supportedFuncs[e.Left.NodeType](e.Left), e.Right),
                            Expression.Multiply(supportedFuncs[e.Right.NodeType](e.Right), e.Left)),
                        Expression.Multiply(e.Right, e.Right));
                },

                [ExpressionType.Add] = (expr) =>
                {
                    var e = (BinaryExpression)expr;
                    return Expression.Add(supportedFuncs[e.Left.NodeType](e.Left), supportedFuncs[e.Right.NodeType](e.Right));
                },

                [ExpressionType.Subtract] = (expr) =>
                {
                    var e = (BinaryExpression)expr;
                    return Expression.Subtract(supportedFuncs[e.Left.NodeType](e.Left), supportedFuncs[e.Right.NodeType](e.Right));
                },

                [ExpressionType.Call] = (expr) =>
                {
                    var e = (MethodCallExpression)expr;
                    if (e.Method.Name == "Sin")
                    {
                        return Expression.Multiply(
                            Expression.Call(null, typeof(Math).GetMethod("Cos", new[] { typeof(double) }), e.Arguments[0]),
                            supportedFuncs[e.Arguments[0].NodeType](e.Arguments[0]));
                    }
                    if (e.Method.Name == "Cos")
                    {
                        return Expression.Multiply(
                            Expression.Multiply(
                                Expression.Call(null, typeof(Math).GetMethod("Sin", new[] { typeof(double) }), e.Arguments[0]),
                                Expression.Constant((double)-1)),
                            supportedFuncs[e.Arguments[0].NodeType](e.Arguments[0]));
                    }
                    throw new ArgumentException(expr.ToString() + " have unexpexted call");
                },

                [ExpressionType.Constant] = (expr) => Expression.Constant((double)0),
                [ExpressionType.Parameter] = (expr) => Expression.Constant((double)1)
            };
        }

        static Differentiator()
        {
            InitializeSupportedFuncs();
        }

        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> funcExpr)
        {
            if (!supportedFuncs.ContainsKey(funcExpr.Body.NodeType))
            {
                throw new ArgumentException(funcExpr.Body.ToString() + " contains unsupported functions");
            }
            var expr = supportedFuncs[funcExpr.Body.NodeType](funcExpr.Body);
            var lambda = Expression.Lambda<Func<double, double>>(expr, funcExpr.Parameters);
            return lambda;
        }
    }
}
