using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionRewriter
{
    public abstract class Visitor
    {
        private readonly Expression _node;
        public ExpressionType NodeType => _node.NodeType;

        protected Visitor(Expression node)
        {
            _node = node;
        }

        public static Visitor CreateFromExpression(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Lambda:
                    return new LambdaVisitor((LambdaExpression)node);
                case ExpressionType.Add:
                case ExpressionType.Equal:
                case ExpressionType.Multiply:
                case ExpressionType.GreaterThan:
                case ExpressionType.AndAlso:
                    return new BinaryVisitor((BinaryExpression)node);
                case ExpressionType.Parameter:
                    return new ParameterVisitor((ParameterExpression)node);
                case ExpressionType.Constant:
                    return new ConstantVisitor((ConstantExpression)node);
                case ExpressionType.Conditional:
                    return new ConditionalVisitor((ConditionalExpression)node);
                case ExpressionType.Call:
                    return new CallVisitor((MethodCallExpression)node);
                case ExpressionType.MemberAccess:
                    return new MemberAccessVisitor((MemberExpression)node);
                default:
                    throw new ArgumentException($"Unknown/unsupported expression node type: ${node.NodeType}.");
            }
        }
        public abstract Expression Visit(string prefix);
        public abstract Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements);
    }

    public class LambdaVisitor : Visitor
    {
        private LambdaExpression _lambdaExpression;

        public LambdaVisitor(LambdaExpression node) : base(node)
        {
            _lambdaExpression = node;
        }

        public override Expression Visit(string prefix)
        {
            Console.WriteLine($"{prefix}{NodeType} with return type {_lambdaExpression.ReturnType} with {_lambdaExpression.Parameters.Count} parameters: ");
            List<ParameterExpression> paramExprs = new List<ParameterExpression>();
            foreach (var param in _lambdaExpression.Parameters)
            {
                paramExprs.Add((ParameterExpression)CreateFromExpression(param).Visit(prefix));
            }
            var body = CreateFromExpression(_lambdaExpression.Body).Visit(prefix + "\t");
            return Expression.Lambda(body, paramExprs);
        }

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements)
        {
            Console.WriteLine($"{prefix} {NodeType} with return type {_lambdaExpression.ReturnType} with {_lambdaExpression.Parameters.Count} parameters: ");
            List<ParameterExpression> paramExprs = new List<ParameterExpression>();
            if (replacements.Count != _lambdaExpression.Parameters.Count)
            {
                throw new Exception("Number of arguments to the .Call method does not match the number of arguments in the underlying delegate.");
            }
            //for (int i = 0; i < _lambdaExpression.Parameters.Count; i++)
            //{
            //	replacements[i].OriginalParameterExpr = _lambdaExpression.Parameters[i];
            //}
            return CreateFromExpression(_lambdaExpression.Body).VisitAndReplace("\t" + prefix, replacements);
        }
    }

    public class BinaryVisitor : Visitor
    {
        private readonly BinaryExpression _binaryExpression;

        public BinaryVisitor(BinaryExpression node) : base(node)
        {
            _binaryExpression = node;
        }

        public override Expression Visit(string prefix)
        {
            Expression leftExpr = CreateFromExpression(_binaryExpression.Left).Visit(prefix + "\t");
            Expression rightExpr = CreateFromExpression(_binaryExpression.Right).Visit(prefix + "\t");
            Expression newBinaryExpr = null;
            Console.WriteLine($"{prefix} {NodeType} with two parameters.");
            switch (_binaryExpression.NodeType)
            {
                case ExpressionType.Add:
                    newBinaryExpr = Expression.Add(leftExpr, rightExpr);
                    break;
                case ExpressionType.Equal:
                    newBinaryExpr = Expression.Equal(leftExpr, rightExpr);
                    break;
                case ExpressionType.Multiply:
                    newBinaryExpr = Expression.Multiply(leftExpr, rightExpr);
                    break;
                case ExpressionType.GreaterThan:
                    newBinaryExpr = Expression.GreaterThan(leftExpr, rightExpr);
                    break;
                case ExpressionType.AndAlso:
                    newBinaryExpr = Expression.AndAlso(leftExpr, rightExpr);
                    break;
            }
            return newBinaryExpr;
        }

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements)
        {
            Expression leftExpr = CreateFromExpression(_binaryExpression.Left).VisitAndReplace("\t" + prefix, replacements);
            Expression rightExpr = CreateFromExpression(_binaryExpression.Right).VisitAndReplace("\t" + prefix, replacements);
            Expression newBinaryExpr = null;
            Console.WriteLine($"{prefix} {NodeType} with two parameters.");
            switch (_binaryExpression.NodeType)
            {
                case ExpressionType.Add:
                    newBinaryExpr = Expression.Add(leftExpr, rightExpr);
                    break;
                case ExpressionType.Equal:
                    newBinaryExpr = Expression.Equal(leftExpr, rightExpr);
                    break;
                case ExpressionType.Multiply:
                    newBinaryExpr = Expression.Multiply(leftExpr, rightExpr);
                    break;
                case ExpressionType.GreaterThan:
                    newBinaryExpr = Expression.GreaterThan(leftExpr, rightExpr);
                    break;
                case ExpressionType.AndAlso:
                    newBinaryExpr = Expression.AndAlso(leftExpr, rightExpr);
                    break;
            }
            return newBinaryExpr;
        }
    }

    public class ParameterVisitor : Visitor
    {
        private readonly ParameterExpression _parameterExpression;

        public ParameterVisitor(ParameterExpression node) : base(node)
        {
            _parameterExpression = node;
        }

        public override Expression Visit(string prefix)
        {
            string isByRef = _parameterExpression.IsByRef ? string.Empty : "not ";
            Console.WriteLine($"{prefix} {NodeType} with parameter '{_parameterExpression.Name}' which is {isByRef}a reference type.");
            return _parameterExpression;
        }

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements)
        {
            Console.WriteLine($"{prefix} {NodeType} with parameter '{_parameterExpression.Name}'.");
            var replacementExpression = replacements
                .Where(r => r.OriginalParameterExpr == _parameterExpression)
                .Select(r => r.ReplacementParameterExpr)
                .First();
            return replacementExpression;
        }
    }

    public class ConstantVisitor : Visitor
    {
        private readonly ConstantExpression _constantExpression;

        public ConstantVisitor(ConstantExpression node) : base(node)
        {
            _constantExpression = node;
        }

        public override Expression Visit(string prefix)
        {
            Console.WriteLine($"{prefix}Constant expression of type {_constantExpression.Type} with value {_constantExpression.Value}.");
            return _constantExpression;
        }

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements)
        {
            return Visit(prefix);
        }
    }

    public class ConditionalVisitor : Visitor
    {
        private readonly ConditionalExpression _conditionalExpression;
        public ConditionalVisitor(ConditionalExpression node) : base(node)
        {
            _conditionalExpression = node;
        }

        public override Expression Visit(string prefix)
        {
            Console.WriteLine($"{prefix} A conditional (ternary) expression");
            Console.WriteLine($"{prefix} Condition:");
            var testExpr = CreateFromExpression(_conditionalExpression.Test).Visit(prefix + "\t");
            Console.WriteLine($"{prefix} If true conditional:");
            var ifTrueExpr = CreateFromExpression(_conditionalExpression.IfTrue).Visit(prefix + "\t");
            Console.WriteLine($"{prefix} If false conditional:");
            var ifFalseExpr = CreateFromExpression(_conditionalExpression.IfFalse).Visit(prefix + "\t");
            return Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);
        }

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements)
        {
            Console.WriteLine($"{prefix} A conditional (ternary) expression");
            Console.WriteLine($"{prefix} Condition:");
            var testExpr = CreateFromExpression(_conditionalExpression.Test).VisitAndReplace("\t" + prefix, replacements);
            Console.WriteLine($"{prefix} If true conditional:");
            var ifTrueExpr = CreateFromExpression(_conditionalExpression.IfTrue).VisitAndReplace("\t" + prefix, replacements);
            Console.WriteLine($"{prefix} If false conditional:");
            var ifFalseExpr = CreateFromExpression(_conditionalExpression.IfFalse).VisitAndReplace("\t" + prefix, replacements);
            return Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);
        }
    }

    public class CallVisitor : Visitor
    {
        private readonly MethodCallExpression _methodCallExpression;

        public CallVisitor(MethodCallExpression node) : base(node)
        {
            _methodCallExpression = node;
        }

        public override Expression Visit(string prefix)
        {
            Console.WriteLine($"{prefix} This is a {NodeType} expression.");
            MethodInfo methodInfo = _methodCallExpression.Method;
            if (!(methodInfo.Name == nameof(ExpressionExtensions.Call) &&
                _methodCallExpression.Method.DeclaringType.Name == nameof(ExpressionExtensions)))
            {
                throw new Exception($"Only {nameof(ExpressionExtensions.Call)} is callable from within an expression tree context.");
            }
            // .Call() is an extension method. 
            // First argument is the method we want to expand.
            // Other arguments are to be passed to the delegate.
            var methodToExpand = (MethodCallExpression)_methodCallExpression.Arguments[0];
            var lambdaToExpand = (LambdaExpression)methodToExpand.Method.Invoke(null, null);
            // The rest of the arguments are meant for the expanded method.
            // We have to replace any parameter in the expanded method's body with 
            // arguments provided to .Call().
            var replacements = new List<ParameterExpressionReplacement>();
            for (int i = 0; i < lambdaToExpand.Parameters.Count; i++)
            {
                var paramToExpand = lambdaToExpand.Parameters[i];
                var expandedMethodParam0 = /*(ParameterExpression) */_methodCallExpression.Arguments[i + 1];
                replacements.Add(new ParameterExpressionReplacement(paramToExpand, expandedMethodParam0));
            }
            //return Expression.GreaterThan(expandedMethodParam0, ((BinaryExpression)exprToExpand.Body).Right);
            return CreateFromExpression(lambdaToExpand).VisitAndReplace(
                "\t[Replacement]" + prefix,
                replacements
            );
        }

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements)
        {
            throw new NotImplementedException();
        }

        private void ExpandMethodParameters(ref ParameterExpression originalNode, ParameterExpression expandedMethodParameter)
        {
            originalNode = expandedMethodParameter;
        }
    }

    public class MemberAccessVisitor : Visitor
    {
        private MemberExpression _memberExpression;

        public MemberAccessVisitor(MemberExpression node) : base(node)
        {
            this._memberExpression = node;
        }

        public override Expression Visit(string prefix) => _memberExpression;

        public override Expression VisitAndReplace(string prefix, List<ParameterExpressionReplacement> replacements) => _memberExpression;
    }

    public class ParameterExpressionReplacement
    {
        public ParameterExpression OriginalParameterExpr { get; set; }
        public /*Parameter*/Expression ReplacementParameterExpr { get; set; }

        public ParameterExpressionReplacement(ParameterExpression originalParameterExpr, /*Parameter*/Expression replacementParameterExpr)
        {
            OriginalParameterExpr = originalParameterExpr;
            ReplacementParameterExpr = replacementParameterExpr;
        }

        public void MapReplacementParameter(ParameterExpression replacementParameterExpr)
        {
            ReplacementParameterExpr = replacementParameterExpr;
        }
    }
}
