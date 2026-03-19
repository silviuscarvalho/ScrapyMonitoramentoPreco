(function () {
    const config = window.monitoramentoDashboard;
    if (!config || !window.signalR) {
        return;
    }

    const atualizarTela = async function () {
        const resposta = await fetch(config.resumoUrl);
        if (!resposta.ok) {
            return;
        }

        const dados = await resposta.json();
        const definirTexto = function (id, valor) {
            const elemento = document.getElementById(id);
            if (elemento) {
                elemento.textContent = valor;
            }
        };

        definirTexto("card-dispositivos", dados.totalDispositivosAtivos);
        definirTexto("card-dns", dados.totalEventosDns24h);
        definirTexto("card-fluxos", dados.totalFluxos24h);
        definirTexto("card-alertas", dados.totalAlertasAbertos);
    };

    const conexao = new signalR.HubConnectionBuilder().withUrl(config.hubUrl).build();
    conexao.on("AtualizarDashboard", atualizarTela);
    conexao.start().catch(function () { });
})();
