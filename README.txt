# Trabalho 1 - IA

Nomes:
- Mateus Carmo (11911BCC026)
- Gustavo Alba (11911BCC016)
- Otávio Leite (11911BCC010)

Execução:
- Para rodar o código fonte, instale o .NET e execute o comando "dotnet run" no diretório raiz.
- Para rodar o executável, certifique-se que o executável está no mesmo diretório que a pasta "assets".

Controles:
- Botão START: inicia a simulação
- Botão R: reseta (e recarrega mapas)
- Botão COSTS: ativa/desativa custos escritos em texto no mapa
- Botões *x : controla velocidade do A*
- Os custos de cada caminho (e do caminho todo, no final) são exibidos abaixo do nome do mapa, no topo da tela!

Detalhes de Implementação:
- As partes mais relevantes ao projeto de IA estão nas classes AStar, World, e Map. Especificamente, AStar contém a lógica do algoritmo A* para encontrar caminhos em um mapa, World contém os dados sobre onde os objetivos e pontos de partida se encontram em cada mapa, além de conter o código de geração do melhor caminho por meio da solução do TSP (a função "FindBestPath"), e a classe Map armazena as tiles e os custos dos mapas e funções/subclasses usadas em todo o projeto para compilar dados relevantes aos dados dos mapas.
- As outras classes são todas, usadas para controlar o aspecto visual do projeto. Por isso, não estão detalhadamente comentadas.
- O aspecto visual foi implementado a biblioteca Raylib, e suas bindings para C# no projeto Raylib_cs de ChrisDill, disponível no GitHub.
