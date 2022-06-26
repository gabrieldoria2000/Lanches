using LanchesMac.Context;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext context)
        {
            _context = context;
        }

        public string CarrinhoCompraId { get; set; }
        public List<CarrinhoCompraItem> CarrinhoCompraItems { get; set; }

        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            //define a sessão - Se a instancia de http context não for nula, ele invoca uma session que iremos usar
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            //obtem uma instancia do contexto
            var context = services.GetService<AppDbContext>();

            //obtem ou gera o ID do carrinho
            //usamos o operador de coalescencia nula ??
            //Um GUID é um inteiro de 128 bits (16 bytes) que pode ser usado em todos os computadores e redes onde
            //quer que um identificador exclusivo seja necessário. 
            String CarrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();

            //atribui o carrinho ID na session
            session.SetString("CarrinhoId", CarrinhoId);

            //retorna o carrinho com o contexto e o ID atribuido ou obtido
            return new CarrinhoCompra(context)
            {
                CarrinhoCompraId = CarrinhoId
            };
        }

        public void AdicionaAoCarrinho(Lanche lanche)
        {
            //verifica se o item existe
            var CarrinhoCompraItem = _context.CarrinhoCompraItens.SingleOrDefault(s=> s.Lanche.LancheId == lanche.LancheId
                                     && s.CarrinhoCompraId == CarrinhoCompraId);

            if (CarrinhoCompraItem == null)
            {
                CarrinhoCompraItem = new CarrinhoCompraItem
                {
                    //obteve da session ou criou
                    CarrinhoCompraId = CarrinhoCompraId,
                    Lanche = lanche,
                    Quantidade = 1
                };
                _context.CarrinhoCompraItens.Add(CarrinhoCompraItem);
            }
            else
            {
                //o carrinho já existe

                CarrinhoCompraItem.Quantidade++;
            }
            _context.SaveChanges();

        }

        public int RemoverDoCarrinho(Lanche lanche)
        {
            //verifica se o item existe
            var CarrinhoCompraItem = _context.CarrinhoCompraItens.SingleOrDefault(s => s.Lanche.LancheId == lanche.LancheId
                                     && s.CarrinhoCompraId == CarrinhoCompraId);

            var quantidadeLocal = 0;

            if( CarrinhoCompraItem != null) 
            {
                if (CarrinhoCompraItem.Quantidade > 1)
                {
                    CarrinhoCompraItem.Quantidade--;
                    quantidadeLocal = CarrinhoCompraItem.Quantidade;
                }
                else
                {
                    _context.CarrinhoCompraItens.Remove(CarrinhoCompraItem);
                }
            }

            _context.SaveChanges();
            return quantidadeLocal;
        }

        public List<CarrinhoCompraItem> GetCarrinhoCompraItens()
        {
            return CarrinhoCompraItems ??
                (CarrinhoCompraItems = _context.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Include(s => s.Lanche).ToList());
        }

        public void LimparCarrinho()
        {
            var carrinhoitens = _context.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId);

            _context.CarrinhoCompraItens.RemoveRange(carrinhoitens);

            _context.SaveChanges();
        }

        public Decimal GetCarrinhoComprasTotal()
        {
            var total = _context.CarrinhoCompraItens
                .Where(c=> c.CarrinhoCompraId == CarrinhoCompraId)
                .Select(c=> c.Lanche.Preco * c.Quantidade).Sum();

            return total;
        }
    }
}
