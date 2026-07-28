<template>
  <div class="landing-container antialiased selection:bg-brand-cyan selection:text-white bg-brand-light text-slate-700">
    <!-- Navbar -->
    <header class="fixed top-0 w-full z-50 transition-all duration-300" :class="navbarClasses" id="navbar">
        <div class="max-w-7xl mx-auto px-6 lg:px-8 h-24 flex justify-between items-center">
            
            <a href="#" class="flex items-center gap-2 !no-underline">
                <span class="material-symbols-outlined text-4xl text-brand-cyan">waves</span>
                <span class="text-2xl font-extrabold !text-brand-navy tracking-tight">EcoWash<span class="text-brand-cyan">.</span></span>
            </a>

            <nav class="hidden md:flex gap-8 items-center font-semibold text-sm !text-brand-navy">
                <a href="#resultados" class="hover:!text-brand-cyan transition-colors !no-underline !text-brand-navy/80 hover:!text-brand-cyan">La Plataforma</a>
                <a href="#proceso" class="hover:!text-brand-cyan transition-colors !no-underline !text-brand-navy/80 hover:!text-brand-cyan">Proceso</a>
                <a href="#paquetes" class="hover:!text-brand-cyan transition-colors !no-underline !text-brand-navy/80 hover:!text-brand-cyan">Paquetes</a>
            </nav>

            <div class="hidden md:flex gap-4 items-center">
                <router-link to="/login" class="border border-brand-navy/20 !text-brand-navy px-6 py-2.5 rounded-full font-bold text-sm hover:bg-brand-navy/5 transition-colors !no-underline">Ingresar</router-link>
                <router-link to="/registro" class="bg-brand-coral hover:bg-orange-500 !text-white px-6 py-2.5 rounded-full font-bold shadow-coral-glow transition-all hover:-translate-y-1 text-sm !no-underline">
                    Crear Cuenta
                </router-link>
            </div>

            <button class="md:hidden text-brand-navy">
                <span class="material-symbols-outlined text-3xl">menu_open</span>
            </button>
        </div>
    </header>

    <main class="pt-24">
        <!-- Hero Section -->
        <section class="relative bg-white pt-12 pb-32 lg:pt-24 lg:pb-40 overflow-hidden">
            <div class="max-w-7xl mx-auto px-6 lg:px-8 flex flex-col lg:flex-row items-center gap-16">
                
                <div class="w-full lg:w-1/2 relative z-10">
                    <div class="inline-flex items-center gap-2 bg-brand-cyan/10 text-brand-cyan font-bold px-4 py-2 rounded-full text-sm mb-6">
                        <span class="material-symbols-outlined text-[18px]">verified</span>
                        El 1er Ecosistema de Detailing en Santa Cruz
                    </div>
                    
                    <h1 class="text-5xl lg:text-7xl font-extrabold text-brand-navy leading-tight mb-6 tracking-tight">
                        Limpieza de lujo, <br>
                        <span class="text-brand-cyan relative">
                            control total.
                            <svg class="absolute w-full h-3 -bottom-1 left-0 text-brand-cyan/30" viewBox="0 0 100 10" preserveAspectRatio="none"><path d="M0 5 Q 50 15 100 5" stroke="currentColor" stroke-width="4" fill="none"/></svg>
                        </span>
                    </h1>
                    
                    <p class="text-lg text-slate-500 mb-10 max-w-lg leading-relaxed font-medium">
                        Llevamos el mejor estudio de detailing directo a la puerta de tu casa.
                    </p>
                    
                    <div class="flex flex-col sm:flex-row gap-4">
                        <router-link :to="getAgendarRoute()" class="!no-underline bg-brand-navy text-white px-8 py-4 rounded-full font-bold text-center hover:bg-brand-navy/90 transition-colors shadow-float flex items-center justify-center gap-2 group">
                            <span class="material-symbols-outlined group-hover:rotate-12 transition-transform">calendar_add_on</span>
                            Agendar Lavado
                        </router-link>
                        <a href="#plataforma" class="bg-brand-light text-brand-navy border border-slate-200 px-8 py-4 rounded-full font-bold text-center hover:bg-slate-100 transition-colors">
                            Conocer la App
                        </a>
                    </div>
                </div>

                <div class="w-full lg:w-1/2 relative">
                    <div class="absolute inset-0 bg-brand-cyan/20 image-mask translate-x-4 translate-y-4"></div>
                    <img src="https://images.unsplash.com/photo-1601362840469-51e4d8d58785?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80" alt="Lavado Ecológico" class="relative z-10 w-full h-[500px] object-cover image-mask shadow-float">
                    
                    <!-- Tarjeta Flotante -->
                    <div class="absolute bottom-10 -left-10 z-20 glass-card p-5 rounded-2xl shadow-float flex items-center gap-4">
                        <div class="w-12 h-12 bg-brand-cyan rounded-full flex items-center justify-center text-white shadow-cyan-glow">
                            <span class="material-symbols-outlined">directions_car</span>
                        </div>
                        <div>
                            <p class="text-sm font-semibold text-slate-500">Unidades Disponibles</p>
                            <p class="text-xl font-bold text-brand-navy">Llegamos en 30 min</p>
                        </div>
                    </div>
                </div>
            </div>

            <div class="wave-divider text-brand-light">
                <svg viewBox="0 0 1200 120" preserveAspectRatio="none">
                    <path d="M321.39,56.44c58-10.79,114.16-30.13,172-41.86,82.39-16.72,168.19-17.73,250.45-.39C823.78,31,906.67,72,985.66,92.83c70.05,18.48,146.53,26.09,214.34,3V120H0V95.8C59.71,118,137.9,122.9,207,105.5,246.3,95.5,284.1,78,321.39,56.44Z" fill="currentColor"></path>
                </svg>
            </div>
        </section>

        <!-- Sección de Resultados (Antes y Después / Interiores) -->
        <section id="resultados" class="py-24 bg-brand-light">
            <div class="max-w-7xl mx-auto px-6 lg:px-8">
                <div class="text-center mb-16">
                    <span class="text-brand-coral font-bold tracking-wider uppercase text-sm">Calidad Garantizada</span>
                    <h2 class="text-4xl md:text-5xl font-extrabold text-brand-navy mt-2 mb-4 tracking-tight">Resultados que hablan por sí mismos</h2>
                    
                </div>

                <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 items-stretch">
                    
                    <!-- Imagen de Antes/Después -->
                    <div class="bg-white rounded-3xl overflow-hidden shadow-float flex flex-col h-full border border-slate-100 group">
                        <div class="relative h-[350px] md:h-[400px] overflow-hidden">
                            <img src="/antes_despues_auto.png" alt="Transformación Exterior Antes y Después" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700">
                            <div class="absolute top-4 left-4 bg-white/90 backdrop-blur-md text-brand-navy text-xs font-bold px-4 py-2 rounded-full shadow flex items-center gap-2 z-10">
                                <span class="material-symbols-outlined text-[16px] text-brand-cyan">compare</span>
                                El Antes y el Después
                            </div>
                        </div>
                        <div class="p-8 flex flex-col flex-grow">
                            <h3 class="text-2xl font-bold text-brand-navy mb-3">Restauración Exterior</h3>
                            
                        </div>
                    </div>

                    <!-- Imagen de Interior -->
                    <div class="bg-white rounded-3xl overflow-hidden shadow-float flex flex-col h-full border border-slate-100 group">
                        <div class="relative h-[350px] md:h-[400px] overflow-hidden">
                            <img src="/interiorauto.png" alt="Limpieza Detallada de Interior" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700">
                            <div class="absolute top-4 left-4 bg-white/90 backdrop-blur-md text-brand-navy text-xs font-bold px-4 py-2 rounded-full shadow flex items-center gap-2 z-10">
                                <span class="material-symbols-outlined text-[16px] text-brand-coral">airline_seat_recline_extra</span>
                                Interiores de Lujo
                            </div>
                        </div>
                        <div class="p-8 flex flex-col flex-grow">
                            <h3 class="text-2xl font-bold text-brand-navy mb-3">Detallado Interior Profundo</h3>
                            
                        </div>
                    </div>

                </div>
            </div>
        </section>

        <!-- Sección de Proceso -->
        <section id="proceso" class="py-24 bg-white relative border-y border-slate-100">
            <div class="max-w-7xl mx-auto px-6 lg:px-8">
                <div class="flex flex-col md:flex-row items-center gap-16">
                    <div class="w-full md:w-1/2">
                        <h2 class="text-4xl font-extrabold text-brand-navy mb-6">Tecnología Ecológica<br><span class="text-brand-cyan">Waterless</span></h2>
                        <p class="text-lg text-slate-500 mb-8">Limpiamos tu auto completo utilizando menos de 2 litros de agua y sin rayar la pintura.</p>
                        
                        <ul class="space-y-6">
                            <li class="flex items-start gap-4">
                                <span class="w-8 h-8 rounded-full bg-brand-cyan/20 text-brand-cyan flex items-center justify-center font-bold flex-shrink-0">1</span>
                                <div>
                                    <h4 class="font-bold text-brand-navy">Selecciona el Servicio</h4>
                                    <p class="text-sm text-slate-500">Desde paquetes exprés hasta tratamientos cerámicos premium.</p>
                                </div>
                            </li>
                            <li class="flex items-start gap-4">
                                <span class="w-8 h-8 rounded-full bg-brand-cyan/20 text-brand-cyan flex items-center justify-center font-bold flex-shrink-0">2</span>
                                <div>
                                    <h4 class="font-bold text-brand-navy">Nuestra Unidad Llega a Ti</h4>
                                    <p class="text-sm text-slate-500">Nuestros empleados asignados llegan equipados con todo lo necesario.</p>
                                </div>
                            </li>
                            <li class="flex items-start gap-4">
                                <span class="w-8 h-8 rounded-full bg-brand-cyan text-white flex items-center justify-center font-bold flex-shrink-0 shadow-cyan-glow">3</span>
                                <div>
                                    <h4 class="font-bold text-brand-navy">Pago y Calificación Digital</h4>
                                    <p class="text-sm text-slate-500">Paga seguro desde la app y califica la experiencia para asegurar nuestra calidad.</p>
                                </div>
                            </li>
                        </ul>
                    </div>
                    <div class="w-full md:w-1/2 relative">
                        <img src="https://images.unsplash.com/photo-1552930294-6b595f4c2974?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" alt="Detailing Profesional" class="rounded-3xl shadow-float w-full h-[500px] object-cover">
                    </div>
                </div>
            </div>
        </section>

        <!-- Catálogo de Servicios (Grid Moderno) -->
        <section id="paquetes" class="py-24 bg-brand-light relative">
            <div class="max-w-7xl mx-auto px-6 lg:px-8 relative z-10">
                <div class="text-center mb-16">
                    <span class="text-brand-cyan font-bold tracking-wider uppercase text-sm">Nuestros Servicios</span>
                    <h2 class="text-4xl md:text-5xl font-extrabold text-brand-navy mt-2 mb-4 tracking-tight">Cuidado detallado para cada necesidad</h2>
                    <p class="text-lg text-slate-500 max-w-2xl mx-auto">Explora nuestro catálogo completo y dale a tu vehículo el cuidado que merece.</p>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                    
                    <!-- Lavado Básico -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-brand-cyan/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <img src="https://images.unsplash.com/photo-1600880292203-757bb62b4baf?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Lavado Básico" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">local_car_wash</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Lavado Básico</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Lavado exterior completo con espuma activa y secado profesional.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 45 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 50</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Lavado Completo -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-brand-cyan/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <img src="https://images.unsplash.com/photo-1517524008697-84bbe3c3fd98?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Lavado Completo" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">directions_car</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Lavado Completo</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Limpieza profunda exterior e interior, incluyendo tableros y vidrios.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 75 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 80</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Lavado Ecológico -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-green-500/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <div class="absolute top-0 right-0 bg-green-500 text-white text-[10px] font-bold px-3 py-1 rounded-bl-xl uppercase tracking-wider z-10">Waterless</div>
                            <img src="https://images.unsplash.com/photo-1552930294-6b595f4c2974?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Lavado Ecológico" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">eco</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Lavado Ecológico</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Lavado en seco con polímeros especiales. Mínimo consumo de agua.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 50 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 65</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Lavado Premium (Destacado con Imagen de Fondo) -->
                    <div class="rounded-3xl shadow-float hover:-translate-y-2 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <img src="https://images.unsplash.com/photo-1601362840469-51e4d8d58785?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" alt="Lavado Premium" class="absolute inset-0 w-full h-full object-cover group-hover:scale-105 transition-transform duration-700">
                        <div class="absolute inset-0 bg-gradient-to-t from-brand-navy via-brand-navy/80 to-brand-navy/30"></div>
                        
                        <div class="absolute top-0 right-0 bg-brand-coral text-white text-[10px] font-bold px-3 py-1 rounded-bl-xl uppercase tracking-wider shadow-coral-glow z-10">Top Ventas</div>
                        
                        <div class="p-6 flex flex-col flex-grow relative z-10 justify-end h-full min-h-[300px]">
                            <div class="w-12 h-12 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center mb-4 border border-white/30">
                                <span class="material-symbols-outlined">diamond</span>
                            </div>
                            <h3 class="text-2xl font-bold text-white mb-2">Lavado Premium</h3>
                            <p class="text-brand-light/80 text-sm mb-4">La experiencia definitiva. Lavado completo, encerado protector y detallado.</p>
                            <div class="flex items-center gap-2 text-brand-light/60 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 120 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-white/20 pt-4 mt-auto">
                                <span class="text-3xl font-extrabold text-white whitespace-nowrap">Bs. 150</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-brand-cyan text-brand-navy px-4 py-2.5 rounded-full font-bold text-sm hover:bg-white transition-colors">
                                    Agendar
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Aspirado -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-brand-cyan/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <img src="https://images.unsplash.com/photo-1553456558-aff63285bdd1?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Aspirado" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">air</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Aspirado</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Aspirado profundo de asientos, alfombras y baúl del vehículo.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 30 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 30</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Encerado -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-orange-500/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <img src="https://images.unsplash.com/photo-1563720223185-11003d516935?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Encerado" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">auto_fix_high</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Encerado</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Aplicación de cera protectora para un brillo duradero y protección UV.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 45 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 60</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Pulido -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-purple-500/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <img src="https://images.unsplash.com/photo-1587560699334-bea93391dcef?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Pulido" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">flare</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Pulido</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Pulido de pintura profesional para eliminar micro-rayones y restaurar el color.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 90 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 120</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                    <!-- Motocicletas -->
                    <div class="bg-white rounded-3xl shadow-sm border border-slate-100 hover:shadow-xl hover:border-red-500/30 transition-all duration-300 group relative overflow-hidden flex flex-col p-0">
                        <div class="h-48 overflow-hidden relative">
                            <img src="https://images.unsplash.com/photo-1558981403-c5f9899a28bc?ixlib=rb-4.0.3&auto=format&fit=crop&w=600&q=80" alt="Lavado de Motos" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
                            <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                            <div class="absolute bottom-4 left-4 w-10 h-10 bg-white/20 backdrop-blur-md text-white rounded-xl flex items-center justify-center border border-white/30">
                                <span class="material-symbols-outlined">two_wheeler</span>
                            </div>
                        </div>
                        <div class="p-6 flex flex-col flex-grow">
                            <h3 class="text-xl font-bold text-brand-navy mb-2">Lavado de Motos</h3>
                            <p class="text-slate-500 text-sm mb-4 flex-grow">Limpieza detallada especial para motocicletas de cualquier cilindrada.</p>
                            <div class="flex items-center gap-2 text-slate-400 text-xs font-medium mb-6">
                                <span class="material-symbols-outlined text-[16px]">schedule</span> 30 mins
                            </div>
                            <div class="flex items-center justify-between border-t border-slate-50 pt-4 mt-auto">
                                <span class="text-2xl font-extrabold text-brand-navy whitespace-nowrap">Bs. 35</span>
                                <router-link :to="getAgendarRoute()" class="!no-underline bg-slate-100 text-brand-navy px-4 py-2 rounded-full font-bold text-sm hover:bg-brand-cyan hover:text-white transition-colors flex items-center gap-2">
                                    Agendar <span class="material-symbols-outlined text-[18px] group-hover:translate-x-1 transition-transform">arrow_forward</span>
                                </router-link>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </section>

        <!-- CTA Final -->
        <section class="py-24 px-6 lg:px-8 bg-brand-navy text-center relative overflow-hidden">
            <div class="absolute top-0 right-0 w-96 h-96 bg-brand-cyan/20 rounded-full blur-[100px] pointer-events-none"></div>
            
            <div class="relative z-10 max-w-3xl mx-auto">
                <span class="material-symbols-outlined text-brand-cyan text-5xl mb-6">workspace_premium</span>
                <h2 class="text-4xl md:text-5xl font-extrabold text-white mb-8 tracking-tight">Crea tu cuenta y transforma el cuidado de tu auto.</h2>
                <router-link to="/registro" class="inline-flex bg-brand-coral text-white px-10 py-4 rounded-full font-bold text-lg hover:bg-orange-500 transition-colors shadow-coral-glow hover:-translate-y-1 transform">
                    Comenzar Ahora
                </router-link>
            </div>
        </section>
    </main>

    <!-- Footer Corporativo -->
    <footer class="bg-brand-dark pt-20 pb-10">
        <div class="max-w-7xl mx-auto px-6 lg:px-8">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-12 mb-16">
                
                <!-- Col 1: Brand & Social -->
                <div class="lg:col-span-1">
                    <a href="#" class="flex items-center gap-2 mb-6">
                        <span class="material-symbols-outlined text-3xl text-brand-cyan">waves</span>
                        <span class="text-2xl font-extrabold text-white tracking-tight">EcoWash<span class="text-brand-cyan">.</span></span>
                    </a>
                    <p class="text-slate-400 text-sm leading-relaxed mb-6 font-medium">
                        El ecosistema tecnológico líder en cuidado automotriz a domicilio. Más que un lavado, una experiencia premium para tu vehículo.
                    </p>
                    <div class="flex gap-3">
                        <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-slate-400 hover:bg-[#1877F2] hover:text-white hover:border-[#1877F2] transition-all">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M22.675 0h-21.35c-.732 0-1.325.593-1.325 1.325v21.351c0 .731.593 1.324 1.325 1.324h11.495v-9.294h-3.128v-3.622h3.128v-2.671c0-3.1 1.893-4.788 4.659-4.788 1.325 0 2.463.099 2.795.143v3.24l-1.918.001c-1.504 0-1.795.715-1.795 1.763v2.312h3.587l-.467 3.622h-3.12v9.293h6.116c.73 0 1.323-.593 1.323-1.325v-21.35c0-.732-.593-1.325-1.325-1.325z"/></svg>
                        </a>
                        <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-slate-400 hover:bg-[#E4405F] hover:text-white hover:border-[#E4405F] transition-all">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zM12 0C8.741 0 8.333.014 7.053.072 2.695.272.273 2.69.073 7.052.014 8.333 0 8.741 0 12c0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98C8.333 23.986 8.741 24 12 24c3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98C15.668.014 15.259 0 12 0zm0 5.838a6.162 6.162 0 100 12.324 6.162 6.162 0 000-12.324zM12 16a4 4 0 110-8 4 4 0 010 8zm6.406-11.845a1.44 1.44 0 100 2.881 1.44 1.44 0 000-2.881z"/></svg>
                        </a>
                        <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-slate-400 hover:bg-black hover:text-white hover:border-black transition-all">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z"/></svg>
                        </a>
                        <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-slate-400 hover:bg-white hover:text-black hover:border-white transition-all">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M19.59 6.69a4.83 4.83 0 01-3.77-4.25V2h-3.45v13.67a2.89 2.89 0 01-5.2 1.74 2.89 2.89 0 012.31-4.64 2.93 2.93 0 01.88.13V9.4a6.84 6.84 0 00-1-.05A6.33 6.33 0 005 15.68l.05.23a6.33 6.33 0 0010.86 4.49A6.35 6.35 0 0015.82 17l-.02-8.52a8.31 8.31 0 004.83 1.54v-3.23a4.79 4.79 0 01-1.04-.1z"/></svg>
                        </a>
                    </div>
                </div>

                <!-- Col 2: Plataforma -->
                <div>
                    <h4 class="text-white font-bold mb-6 tracking-wide">La Plataforma</h4>
                    <ul class="list-none m-0 p-0 space-y-3 text-slate-400 text-sm font-medium">
                        <li><router-link to="/login" class="hover:text-brand-cyan transition-colors flex items-center gap-2"><span class="material-symbols-outlined text-[16px]">chevron_right</span> Portal de Cliente</router-link></li>
                        <li><router-link to="/registro" class="hover:text-brand-cyan transition-colors flex items-center gap-2"><span class="material-symbols-outlined text-[16px]">chevron_right</span> Regístrate Gratis</router-link></li>
                        <li><router-link to="/login" class="hover:text-brand-cyan transition-colors flex items-center gap-2"><span class="material-symbols-outlined text-[16px]">chevron_right</span> Acceso Empleados</router-link></li>
                        <li><router-link to="/login" class="hover:text-brand-cyan transition-colors flex items-center gap-2"><span class="material-symbols-outlined text-[16px]">chevron_right</span> Panel de Administración</router-link></li>
                    </ul>
                </div>

                <!-- Col 3: Contacto -->
                <div>
                    <h4 class="text-white font-bold mb-6 tracking-wide">Contáctanos</h4>
                    <ul class="list-none m-0 p-0 space-y-4 text-slate-400 text-sm font-medium">
                        <li class="flex items-start gap-3">
                            <span class="material-symbols-outlined text-brand-cyan text-[20px]">location_on</span>
                            <span>Av. Banzer Km 5, 3er Anillo Interno.<br>Santa Cruz de la Sierra, Bolivia</span>
                        </li>
                        <li class="flex items-center gap-3">
                            <span class="material-symbols-outlined text-brand-cyan text-[20px]">call</span>
                            <span>+591 77000000</span>
                        </li>
                        <li class="flex items-center gap-3">
                            <span class="material-symbols-outlined text-brand-cyan text-[20px]">mail</span>
                            <a href="mailto:contacto@ecowash.bo" class="hover:text-brand-cyan transition-colors">contacto@ecowash.bo</a>
                        </li>
                    </ul>
                    
                </div>

                <!-- Col 4: Atención -->
                <div>
                    <h4 class="text-white font-bold mb-6 tracking-wide">¿Necesitas ayuda rápida?</h4>
                    <p class="text-slate-400 text-sm mb-6 font-medium leading-relaxed">
                        Nuestro equipo de soporte está disponible de 08:00 a 20:00 para resolver tus dudas o ayudarte a agendar.
                    </p>
                    <a href="https://wa.me/59177000000" target="_blank" class="inline-flex items-center gap-2 bg-[#25D366] border border-[#25D366] text-white font-bold px-6 py-3 rounded-xl hover:bg-[#1DA851] hover:border-[#1DA851] shadow-lg shadow-[#25D366]/20 transition-all w-full justify-center group hover:-translate-y-1">
                        <svg class="w-5 h-5 group-hover:scale-110 transition-transform" fill="currentColor" viewBox="0 0 24 24"><path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51a12.8 12.8 0 0 0-.57-.01c-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 0 1-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 0 1-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.82 9.82 0 0 1 2.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0 0 12.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 0 0 5.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 0 0-3.48-8.413Z"/></svg>
                        Escribir al WhatsApp
                    </a>
                </div>
            </div>

            <!-- Mapa de Ubicación (Acomodado) -->
            <div class="w-full h-48 md:h-64 rounded-2xl overflow-hidden shadow-lg border border-slate-800 relative mb-12 group">
                <div class="absolute inset-0 bg-brand-cyan/5 pointer-events-none z-10"></div>
                <iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d121544.75549046033!2d-63.26244498305007!3d-17.814581977793576!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x93f1e7d32c943be7%3A0xb30495392e2c452e!2sSanta%20Cruz%20de%20la%20Sierra!5e0!3m2!1sen!2sbo!4v1700000000000!5m2!1sen!2sbo" width="100%" height="100%" style="border:0; filter: contrast(1.1);" allowfullscreen="" loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe>
            </div>

            <!-- Footer Bottom -->
            <div class="border-t border-slate-800 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-slate-500 text-sm">
                <p>&copy; 2026 EcoWash Móvil. Desarrollado por Programación Web II | Universidad Privada Domingo Savio.</p>
                <div class="flex gap-4">
                    <a href="#" class="hover:text-brand-cyan transition-colors">Términos del Sistema</a>
                    <span class="text-slate-700">|</span>
                    <a href="#" class="hover:text-brand-cyan transition-colors">Políticas de Privacidad</a>
                </div>
            </div>
        </div>
    </footer>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const getAgendarRoute = () => {
  if (authStore.isAuthenticated && authStore.rol === 'Cliente') {
    return '/cliente/reservar'
  }
  return '/login'
}

const navbarClasses = ref(['bg-white/85', 'shadow-soft'])

const handleScroll = () => {
  if (window.scrollY > 20) {
    navbarClasses.value = ['bg-white/95', 'shadow-md']
  } else {
    navbarClasses.value = ['bg-white/85', 'shadow-soft']
  }
}

onMounted(() => {
  window.addEventListener('scroll', handleScroll)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.landing-container {
  background-color: #f4f7f6;
  color: #334155;
  font-family: 'Plus Jakarta Sans', sans-serif;
}

.image-mask {
  border-radius: 30% 70% 70% 30% / 30% 30% 70% 70%;
}

.wave-divider {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  overflow: hidden;
  line-height: 0;
  transform: translateY(1px);
}
.wave-divider svg {
  display: block;
  width: calc(100% + 1.3px);
  height: 70px;
}

.glass-card {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255,255,255,0.5);
}

/* Ensure global box-sizing and resetting if not already applied by tailwind */
*, *::before, *::after {
  box-sizing: border-box;
}
</style>
