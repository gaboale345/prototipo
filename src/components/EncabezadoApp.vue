<!-- //defineprops()
// declara los datos que el componente puede recibir desde su componente padre.
//Vue exige declarar explicitamente cada propiedad que recibia el componente  -->
<script setup>
defineProps({

    titulo:{
        type:String,
        required:true
    },
    subtitulo:{
        type:String,
        default:''
    }
})
</script>
    
<template> 
    <header class="encabezado">
        <h1>{{titulo}}</h1>
        <p>{{subtitulo}} </p>
    </header>
</template>

<style scoped>
.encabezado {
padding:24px;
background: #1f2937;
color:white;
text-align: center;
}
.encabezado h1 {
    margin: 0;
}
.encabezado p {
    margin-bottom: 0;
}
</style>