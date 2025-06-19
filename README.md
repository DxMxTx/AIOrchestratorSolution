# AIOrchestratorSolution

Pequeña aplicación para aprender el framework de Microsoft sobre planificación de agentes Semantic Kernel.

La aplicación está creada en Blazor y como hemos comentado utiliza Semantic Kernel con su funcionalidad de agentes. 

La aplicación está creada para hacer pequeñas webs con la ayuda de diferentes agentes de IA. Cada agente o plugin tiene una funcionalidad. 

AGENTES CREADOS PARA REALIZAR LA OPERATIVA 

-WebAnalyzerPlugin -> Agente formal diseñado en una clase de C#, donde el usuario le puede indicar a la aplicación si quiere una inspiración de una web en concreto

-DesignPlugin -> Agente encargado en el diseño de la web. A partir de la información que le ha pasado el WebAnalyzerPlugin y la información del usuario diseñará los requisitos de la web

-StylingPlugin -> Agente encargado en el estilo que tendrá la web

-CodePlugin -> Agente encargado en la creación de la web desde 0 a partir de las directrices que le darán los agentes anteriores

-RefinementPlugin -> Agente encargado, en operar después de la creación de la web en hacer las modificaciones que le pida el usuario. No creará el código desde 0, siempre hará modificaciones
