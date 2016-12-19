using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;

namespace Takenet.Textc.Samples
{
    public class Pizza
    {
        private static long _globalOrderId;
        private readonly Dictionary<long, Order> _orderDictionary = new Dictionary<long, Order>();

        public Task<string> ConfirmOrderAsync(string size, string flavor, string address, IRequestContext context)
        {
            var order = new Order
            {
                Size = size,
                Flavor = flavor,
                Address = address
            };
            var orderId = SaveOrder(order);

            var builder = new StringBuilder();
            builder.AppendLine("Seu pedido:");
            builder.AppendLine($"- Sabor: {flavor}");
            builder.AppendLine($"- Tamanho: {size}");
            builder.AppendLine($"- Endereço para entrega: {address}");
            builder.Append("Você confirma?");

            context.SetVariable(nameof(orderId), orderId);

            return Task.FromResult(builder.ToString());
        }

        public Task<string> ProcessOrderAsync(long orderId, IRequestContext context)
        {
            var order = GetOrder(orderId);
            if (order == null)
            {
                return Task.FromResult("Ops, não encontrei o pedido solicitado :(");
            }
            DeleteOrder(orderId);

            context.SetVariable("size", order.Size);
            context.SetVariable("flavor", order.Flavor);
            context.SetVariable("address", order.Address);

            context.RemoveVariable(nameof(orderId));
            
            var builder = new StringBuilder();
            builder.AppendLine("Seu pedido foi realizado com sucesso!");
            builder.Append("Ah, salvamos suas preferências para os próximos pedidos :)");
            return Task.FromResult(builder.ToString());
        }

        public Task<string> CancelOrderAsync(long orderId, IRequestContext context)
        {
            DeleteOrder(orderId);
            context.Clear();
            return Task.FromResult("O pedido foi cancelado e suas preferências removidas");
        }

        private long SaveOrder(Order order)
        {
            var orderId = ++_globalOrderId;
            _orderDictionary.Add(orderId, order);
            return orderId;
        }

        private Order GetOrder(long orderId)
        {
            Order order;
            _orderDictionary.TryGetValue(orderId, out order);
            return order;
        }

        private void DeleteOrder(long orderId)
        {
            _orderDictionary.Remove(orderId);
        }

        public static ITextProcessor CreateTextProcessor()
        {
            // The parsed syntaxes
            var confirmOrderSyntax1 = CsdlParser.Parse(
                ":LDWord?(quero,mande,solicito) :Word?(uma) :Word?(pizza) :Word?(do,no) :LDWord?(tamanho) size:LDWord(pequena,media,média,grande,gigante) :Word?(sabor,de) flavor:LDWord(marguerita,pepperoni,calabreza) :Word?(para) :Word?(à,a,o) address:Text");

            var confirmOrderSyntax2 = CsdlParser.Parse(
                ":LDWord?(quero,mande,solicito) :Word?(uma) :Word?(pizza) :Word?(sabor,de) flavor:LDWord(marguerita,pepperoni,calabreza) :Word?(do,no) :LDWord?(tamanho) size:LDWord(pequena,media,média,grande,gigante) :Word?(para) :Word?(à,a,o) address:Text");
            
            var processOrderSyntax = CsdlParser.Parse(
                ":Word(sim) orderId:Long");
            var cancelOrderSyntax = CsdlParser.Parse(
                ":Word(nao,não) orderId:Long");

            // The output processor handles the command method return value 
            var addReminderOutputProcessor = new DelegateOutputProcessor<string>(
                (text, context) => Console.WriteLine(text));

            var pizza = new Pizza();
            var confirmOrderCommandProcessor = new ReflectionCommandProcessor(
                pizza,
                nameof(ConfirmOrderAsync),
                true,
                addReminderOutputProcessor,
                confirmOrderSyntax1,
                confirmOrderSyntax2);
            var processOrderCommandProcessor2 = new ReflectionCommandProcessor(
                pizza,
                nameof(ProcessOrderAsync),
                true,
                addReminderOutputProcessor,
                processOrderSyntax);
            var cancelOrderCommandProcessor = new ReflectionCommandProcessor(
                pizza,
                nameof(CancelOrderAsync),
                true,
                addReminderOutputProcessor,
                cancelOrderSyntax);

            // Register the the processor
            var textProcessor = new TextProcessor();
            textProcessor.CommandProcessors.Add(confirmOrderCommandProcessor);
            textProcessor.CommandProcessors.Add(processOrderCommandProcessor2);
            textProcessor.CommandProcessors.Add(cancelOrderCommandProcessor);

            // Add some preprocessors to normalize the input text
            textProcessor.TextPreprocessors.Add(new TextNormalizerPreprocessor());
            textProcessor.TextPreprocessors.Add(new ToLowerCasePreprocessor());

            return textProcessor;
        }
    }

    public class Order
    {
        public string Size { get; set; }

        public string Flavor { get; set; }

        public string Address { get; set; }
    }
}
