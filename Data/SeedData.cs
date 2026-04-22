namespace Data;

public static class SeedData
{
	public static async Task SeedAsync(AcademicPlannerDBContext context)
	{
		List<(string SubjectName, string Description, string Color, int SortOrder, List<(string Name, int SortOrder)> Topics)> subjectDefinitions = new()
		{
			(
				"Español",
				"Comprensión lectora, uso del lenguaje, ortografía, redacción y morfosintaxis",
				"#E74C3C",
				1,
				new()
				{
					("Comprensión lectora: identificación de ideas principales y secundarias", 1),
					("Comprensión lectora: inferencias y propósito comunicativo", 2),
					("Tipos de texto y sus características: narrativo, descriptivo, argumentativo y expositivo", 3),
					("Coherencia y cohesión textual: conectores y organización del texto", 4),
					("Vocabulario en contexto: significado, sinonimia y antonimia", 5),
					("Ortografía: acentuación prosódica, ortográfica y diacrítica", 6),
					("Ortografía: uso de mayúsculas y signos de puntuación", 7),
					("Redacción: estructura del párrafo y organización de ideas", 8),
					("Morfología: categorías gramaticales (sustantivo, adjetivo, verbo, adverbio, pronombre, preposición)", 9),
					("Sintaxis: análisis de oraciones simples y compuestas", 10),
					("Comunicación: funciones del lenguaje y actos comunicativos", 11),
				}
			),
			(
				"Literatura",
				"Géneros literarios, figuras retóricas, literatura mexicana y universal",
				"#E91E63",
				2,
				new()
				{
					("El texto literario: características, funciones y contexto", 1),
					("Géneros literarios: narrativa (cuento, novela, crónica, epopeya)", 2),
					("Géneros literarios: lírica (poesía, oda, elegía, soneto)", 3),
					("Géneros literarios: drama (tragedia, comedia, tragicomedia)", 4),
					("Géneros literarios: ensayo y sus características", 5),
					("Figuras retóricas: metáfora, símil, hipérbole, personificación, anáfora", 6),
					("Literatura prehispánica: cosmovisión y textos representativos (Popol Vuh, Chilam Balam)", 7),
					("Literatura colonial mexicana: Sor Juana Inés de la Cruz y el Barroco", 8),
					("Literatura del siglo XIX mexicano: Romanticismo y Modernismo", 9),
					("Literatura del siglo XX mexicano: Contemporáneos, Generación del 50 y narrativa moderna", 10),
					("Literatura hispanoamericana: Boom latinoamericano (García Márquez, Rulfo, Fuentes, Paz)", 11),
					("Literatura universal: Antigua y Medieval (Homero, Dante, Virgilio)", 12),
					("Literatura universal: Renacimiento y Barroco (Cervantes, Shakespeare)", 13),
					("Literatura universal: siglos XIX-XX (Romantismo, Realismo, Vanguardias)", 14),
				}
			),
			(
				"Matemáticas",
				"Aritmética, álgebra, geometría, trigonometría, cálculo, probabilidad y estadística",
				"#3498DB",
				3,
				new()
				{
					("Conjuntos numéricos: operaciones con números reales, enteros y racionales", 1),
					("Proporcionalidad: razones, proporciones y porcentajes", 2),
					("Álgebra: expresiones algebraicas, polinomios y factorización", 3),
					("Álgebra: ecuaciones de primer y segundo grado (cuadráticas)", 4),
					("Álgebra: sistemas de ecuaciones lineales", 5),
					("Sucesiones y series: progresiones aritméticas y geométricas", 6),
					("Geometría plana: ángulos, triángulos, polígonos y el círculo", 7),
					("Geometría: áreas y perímetros de figuras planas", 8),
					("Geometría: áreas y volúmenes de cuerpos geométricos", 9),
					("Geometría analítica: plano cartesiano, recta y cónicas", 10),
					("Trigonometría: razones trigonométricas y la circunferencia unitaria", 11),
					("Trigonometría: identidades y resolución de triángulos", 12),
					("Cálculo diferencial: límites y derivadas", 13),
					("Probabilidad: espacio muestral, eventos y regla de Laplace", 14),
					("Estadística descriptiva: medidas de tendencia central y dispersión", 15),
				}
			),
			(
				"Biología",
				"Célula, bioquímica, genética, evolución, ecología y fisiología humana",
				"#27AE60",
				4,
				new()
				{
					("Características de los seres vivos y niveles de organización biológica", 1),
					("Bioquímica: moléculas orgánicas (carbohidratos, lípidos, proteínas y ácidos nucleicos)", 2),
					("La célula: teoría celular, diferencias entre célula procariota y eucariota", 3),
					("La célula: estructura y función de organelos (núcleo, mitocondria, ribosoma, etc.)", 4),
					("División celular: mitosis y su importancia en el crecimiento", 5),
					("División celular: meiosis y su relación con la reproducción sexual", 6),
					("Metabolismo: fotosíntesis (anabolismo) y sus etapas", 7),
					("Metabolismo: respiración celular (catabolismo) aerobia y anaerobia", 8),
					("Herencia genética: Leyes de Mendel y tipos de herencia", 9),
					("Genética molecular: estructura del ADN, replicación, transcripción y traducción", 10),
					("Biotecnología: ingeniería genética y sus aplicaciones", 11),
					("Evolución biológica: teorías de Darwin-Wallace y evidencias evolutivas", 12),
					("Biodiversidad: taxonomía, los cinco reinos y criterios de clasificación", 13),
					("Ecología: ecosistemas, cadenas y redes tróficas, flujo de energía", 14),
					("Ciclos biogeoquímicos: ciclo del carbono, nitrógeno y agua", 15),
					("Anatomía y fisiología: sistema digestivo y nutrición", 16),
					("Anatomía y fisiología: sistema circulatorio y respiratorio", 17),
					("Anatomía y fisiología: sistema nervioso, endocrino e inmunológico", 18),
					("Anatomía y fisiología: sistema reproductor y desarrollo embrionario", 19),
				}
			),
			(
				"Química",
				"Estructura atómica, tabla periódica, enlace, reacciones, soluciones y química orgánica",
				"#9B59B6",
				5,
				new()
				{
					("La materia: propiedades, estados de agregación y cambios de estado", 1),
					("Estructura atómica: modelos atómicos (Dalton, Thomson, Rutherford, Bohr, mecánico-cuántico)", 2),
					("Estructura atómica: números cuánticos y configuración electrónica", 3),
					("Tabla periódica: organización, grupos, períodos y propiedades periódicas", 4),
					("Enlace químico: iónico, covalente (polar y apolar) y metálico", 5),
					("Nomenclatura química inorgánica: óxidos, hidróxidos, ácidos y sales", 6),
					("Reacciones químicas: tipos (síntesis, descomposición, sustitución, doble sustitución) y balanceo", 7),
					("Estequiometría: concepto de mol, masa molar y cálculos estequiométricos", 8),
					("Soluciones: concentración, propiedades coligativas y preparación", 9),
					("Ácidos y bases: teorías de Arrhenius y Brønsted-Lowry, pH y neutralización", 10),
					("Electroquímica: reacciones redox, pilas electroquímicas y electrólisis", 11),
					("Termoquímica: entalpía, Ley de Hess y energía en las reacciones", 12),
					("Cinética química y equilibrio químico: Ley de acción de masas", 13),
					("Química orgánica: hidrocarburos (alcanos, alquenos, alquinos y aromáticos)", 14),
					("Química orgánica: grupos funcionales (alcoholes, aldehídos, cetonas, ácidos carboxílicos)", 15),
				}
			),
			(
				"Física",
				"Mecánica, termodinámica, ondas, óptica y electromagnetismo",
				"#E67E22",
				6,
				new()
				{
					("Magnitudes físicas, sistemas de unidades, notación científica y vectores", 1),
					("Cinemática: movimiento rectilíneo uniforme (MRU) y uniformemente acelerado (MRUA)", 2),
					("Cinemática: movimiento parabólico y movimiento circular", 3),
					("Dinámica: Leyes de Newton y tipos de fuerzas (fricción, normal, tensión)", 4),
					("Trabajo, potencia y energía mecánica; Ley de conservación de la energía", 5),
					("Impulso, cantidad de movimiento y colisiones", 6),
					("Gravitación universal: Ley de Newton y movimiento planetario", 7),
					("Termodinámica: temperatura, calor, escalas termométricas y calorimetría", 8),
					("Termodinámica: Leyes de la termodinámica y máquinas térmicas", 9),
					("Ondas mecánicas: propiedades (amplitud, frecuencia, longitud de onda) y el sonido", 10),
					("Óptica geométrica: reflexión, refracción, lentes y espejos", 11),
					("Electrostática: Ley de Coulomb, campo eléctrico y potencial eléctrico", 12),
					("Corriente eléctrica, resistencia y Ley de Ohm; circuitos en serie y paralelo", 13),
					("Magnetismo: campo magnético, fuerza magnética e inducción electromagnética", 14),
				}
			),
			(
				"Historia de México",
				"Desde el México prehispánico hasta el México contemporáneo",
				"#C0392B",
				7,
				new()
				{
					("México prehispánico: Mesoamérica y sus principales civilizaciones (Olmecas, Mayas, Aztecas)", 1),
					("La Conquista española: causas, desarrollo y caída del Imperio Azteca (1519-1521)", 2),
					("El Virreinato de Nueva España: organización política, económica y social (siglos XVI-XVIII)", 3),
					("La Iglesia y la cultura colonial: evangelización, arte e Inquisición", 4),
					("Crisis del Virreinato: antecedentes de la Independencia e Ilustración", 5),
					("La Independencia de México: etapas, protagonistas y consumación (1810-1821)", 6),
					("El México independiente: Constitución de 1824, inestabilidad política e intervención extranjera", 7),
					("La Reforma: Benito Juárez, Leyes de Reforma y Guerra de los Tres Años (1855-1861)", 8),
					("La intervención francesa y el Segundo Imperio de Maximiliano (1862-1867)", 9),
					("La República Restaurada (1867-1876) y el inicio del Porfiriato", 10),
					("El Porfiriato: modernización, inversión extranjera y desigualdad social (1876-1910)", 11),
					("La Revolución Mexicana: causas, etapas, protagonistas y Constitución de 1917", 12),
					("El México posrevolucionario: caudillismo, Maximato y la formación del PNR", 13),
					("El cardenismo: reforma agraria, expropiación petrolera y educación socialista (1934-1940)", 14),
					("El 'milagro mexicano': industrialización, estabilidad política y populismo (1940-1970)", 15),
					("Crisis económicas y movimiento estudiantil del 68; apertura democrática", 16),
					("La transición democrática y el México contemporáneo (1988-actualidad)", 17),
				}
			),
			(
				"Historia Universal",
				"Desde la prehistoria hasta el mundo contemporáneo",
				"#8E44AD",
				8,
				new()
				{
					("Prehistoria: hominización y las primeras sociedades humanas", 1),
					("Las civilizaciones fluviales: Mesopotamia, Egipto y el Mediterráneo oriental", 2),
					("La civilización griega: polis, democracia, cultura y filosofía", 3),
					("Roma: República, Imperio, caída y legado cultural", 4),
					("La Edad Media: feudalismo, Iglesia, cruzadas y el Islam", 5),
					("El Renacimiento: humanismo, arte, ciencia y las grandes exploraciones", 6),
					("La Reforma protestante y la Contrarreforma (siglo XVI)", 7),
					("El absolutismo monárquico y la Revolución científica (siglos XVII)", 8),
					("La Ilustración y el Despotismo ilustrado (siglo XVIII)", 9),
					("La Independencia de los Estados Unidos (1776) y la Revolución Francesa (1789)", 10),
					("Napoleón Bonaparte y la reconfiguración de Europa (1799-1815)", 11),
					("La Revolución Industrial: causas, características e impacto social", 12),
					("El imperialismo y el colonialismo europeo en África y Asia (siglo XIX)", 13),
					("La Primera Guerra Mundial: causas, desarrollo y consecuencias (1914-1918)", 14),
					("La Revolución rusa y el surgimiento del comunismo (1917)", 15),
					("El período de entreguerras: la Gran Depresión, fascismo y nazismo (1919-1939)", 16),
					("La Segunda Guerra Mundial: causas, desarrollo y el Holocausto (1939-1945)", 17),
					("La Guerra Fría: bloques, conflictos regionales y carrera armamentista (1947-1991)", 18),
					("Descolonización y el Tercer Mundo (1945-1970)", 19),
					("El mundo contemporáneo: globalización, caída del muro de Berlín y nuevos retos", 20),
				}
			),
			(
				"Geografía",
				"Geografía física, humana, económica y de México",
				"#1ABC9C",
				9,
				new()
				{
					("La geografía: objeto de estudio, ramas y relación con otras ciencias", 1),
					("La Tierra: forma, movimientos (rotación y traslación) y sus consecuencias", 2),
					("Cartografía: mapas, escalas, proyecciones cartográficas y orientación", 3),
					("La litosfera: estructura interna de la Tierra, tectónica de placas, volcanes y sismos", 4),
					("El relieve terrestre: continentes, montañas, llanuras y formas del relieve marino", 5),
					("La atmósfera: composición, capas y fenómenos meteorológicos", 6),
					("El clima: elementos, factores y clasificación de los climas del mundo", 7),
					("La hidrosfera: océanos, mares, ríos, lagos y aguas subterráneas", 8),
					("La biosfera: biomas terrestres y acuáticos", 9),
					("Geografía de la población: crecimiento, distribución, natalidad, mortalidad y migración", 10),
					("Urbanización: ciudades, metropolización y problemas urbanos", 11),
					("Los recursos naturales y las actividades económicas: agricultura, minería, industria y servicios", 12),
					("Medio ambiente: problemas ambientales globales (cambio climático, deforestación) y desarrollo sustentable", 13),
					("Geografía de México: relieve, climas, hidrografía y regiones naturales", 14),
					("Geografía de México: población, distribución territorial, ciudades y regiones económicas", 15),
				}
			)
		};

		foreach ((string subjectName, string description, string color, int sortOrder, List<(string Name, int SortOrder)> topicDefs) in subjectDefinitions)
		{
			Subject? subject = await context.Subjects.FirstOrDefaultAsync(s => s.Name == subjectName);
			if (subject == null)
			{
				subject = new Subject
				{
					Name = subjectName,
					Description = description,
					Color = color,
					SortOrder = sortOrder
				};
				await context.Subjects.AddAsync(subject);
				await context.SaveChangesAsync();
			}

			foreach ((string topicName, int topicSortOrder) in topicDefs)
			{
				bool topicExists = await context.Topics.AnyAsync(t => t.SubjectId == subject.Id && t.Name == topicName);
				if (!topicExists)
				{
					await context.Topics.AddAsync(new Topic
					{
						SubjectId = subject.Id,
						Name = topicName,
						SortOrder = topicSortOrder,
						Status = Helper.STATUS_PENDING
					});
				}
			}

			await context.SaveChangesAsync();
		}
	}
}
